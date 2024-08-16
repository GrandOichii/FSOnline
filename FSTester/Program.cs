using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using FSCore.Cards.CardMasters;
using FSCore.Matches;
using FSCore.Matches.Players.Controllers;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FSTester;

public class Program {
    private static MatchConfig ReadConfigYAML(string text) {
        var deserializer = new DeserializerBuilder()
            // .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<MatchConfig>(text);
    }

    private static MatchConfig ReadConfigJSON(string text) {
        return JsonSerializer.Deserialize<MatchConfig>(text);
    }

    public static async Task TcpMatch(MatchConfig config, int playerCount, int realPlayerCount) {
        var cm = new FileCardMaster();
        cm.Load("../cards/testing");

        var address = IPAddress.Any;
        int port = 9090;
        var endpoint = new IPEndPoint(address, port);
        TcpListener listener = new(endpoint);
        listener.Start();

        var seed = 0;
        // seed = new Random().Next();
        
        var match = new Match(config, seed, cm, File.ReadAllText("../core.lua")){
            Logger = LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger("Match")
        };

        for (int i = 0; i < realPlayerCount; i++) {
            await AddTCPPlayer(listener, match);
            // await AddConsolePlayer(match);
        }

        for (int i = 0; i < playerCount - realPlayerCount; i++) {
            await AddRandomPlayer(match);
            // await AddConsolePlayer(match);
        }

        try {
            await match.Run();
        } catch (Exception e) {
            PrintException(e);
        } finally {
            listener.Stop();
        }
    }

    public static async Task AddRandomPlayer(Match match) {
        var c = new RandomPlayerController(0, 50);

        string name = "RandomPlayer";
        // TODO prompt player name with default name
        // TODO prompt player with character key

        // var chKey = "guppy-v2";

        await match.AddPlayer(name, c);
    }

    public static async Task AddConsolePlayer(Match match) {
        var c = new ConsolePlayerController();

        string name = "ConsolePlayer";
        // TODO prompt player name with default name
        // TODO prompt player with character key

        var chKey = "the-keeper-v2";

        await match.AddPlayer(name, c, chKey);
    }

    public static async Task AddTCPPlayer(TcpListener listener, Match match) {
        System.Console.WriteLine("Waiting for connection...");
        var client = new TcpIOHandler(listener.AcceptTcpClient());

        // read name 
        System.Console.WriteLine("Connection established, reading name...");
        var name = await client.Read();
        // var name = "TcpPlayer";

        System.Console.WriteLine("Name received, reading character key...");
        var chKey = await client.Read();
        // TODO validate

        System.Console.WriteLine($"Read name {name}, creating controller");
        var controller = new IOPlayerController(client);
        await match.AddPlayer(name, controller, chKey);
    }

    public static async Task TcpLoop(MatchConfig config) {
        while (true) {
            try {
                await TcpMatch(config, 3, 1);
                // return;
            } catch (Exception e) {
                PrintException(e);
                // return;
            }
        }
    }

    public static async Task TestRandom(int start, int end, bool enableLogging = false) {
        var config = ReadConfigJSON(
            File.ReadAllText("../configs/test.json")
        );

        var cm = new FileCardMaster();
        cm.Load("../cards/testing");

        for (int i = start; i <= end; i++) {
            System.Console.WriteLine("Seed: " + i);
            var match = new Match(config, i, cm, File.ReadAllText("../core.lua"));
            if (enableLogging)
                match.Logger = LoggerFactory
                    .Create(builder => builder.AddConsole())
                    .CreateLogger("Match");
            await AddRandomPlayer(match);
            await AddRandomPlayer(match);
            try {
                await match.Run();
            } catch (Exception e) {
                PrintException(e);
                
                return;
            }
        }
    }

    public static async Task GenerateSql(string[] args) {
        var dirs = new List<string>();
        for (int i = 2; i < args.Length; i++)
            dirs.Add(args[i]);

        var cm = new FileCardMaster();
        foreach (var dir in dirs) 
            cm.Load(dir);

        var result = "";
        var keys = await cm.GetKeys();
        foreach (var key in keys) {
            var card = await cm.Get(key);
            var set = key.Split("-").Last();
            var name = card.Name
                .Replace("\'", "\'\'")
            ;
            var text = card.Text
                .Replace("\'", "\'\'")
            ;
            var script = card.Script
                // .Replace("\n", "\\n")
                .Replace("\'", "\'\'")
            ;
            var row = $"INSERT INTO card_models (key, name, type, text, script, set) VALUES (\'{card.Key}\',\'{name}\',\'{card.Type}\',\'{text}\',\'{script}\',\'{set}\');\n";
            result += row;
        }

        File.WriteAllText(args[1], result);
    }

    public static async Task Main(string[] args) {
        if (args.Length > 2 && args[0] == "sql") {
            await GenerateSql(args);
            return;
        }

        var seed = -1;
        if (args.Length >= 1) {
            seed = int.Parse(args[0]);
        }
        if (args.Length >= 2) {
            var log = false;
            if (args.Length == 3) log = bool.Parse(args[2]);
            await TestRandom(seed, int.Parse(args[1]), log);
            return;
        }

        var config = ReadConfigYAML(
            File.ReadAllText("../configs/base.json")
        );

        // await TcpLoop(config);
        // return;

        // await TcpMatch(config, 2, 0);
        // return;

        var cm = new FileCardMaster();
        cm.Load("../cards/testing");

        var c1 = new ConsolePlayerController();
        var c2 = c1;

        var match = new Match(config, 0, cm, File.ReadAllText("../core.lua"));
        match.Logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger("Match");

        await match.AddPlayer("player1", c1);
        await AddRandomPlayer(match);
        
        try {
            await match.Run();
        } catch (Exception e) {
            PrintException(e);
            
            return;
        }
    }

    static void PrintException(Exception e) {
        var ex = e;
        while (ex is not null) {
            System.Console.WriteLine(ex.ToString());
            ex = ex.InnerException;
        }
    }
}
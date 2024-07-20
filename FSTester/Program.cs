using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using FSCore.Matches;
using FSCore.Matches.Players.Controllers;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FSTester;

public class ConsolePlayerController : IPlayerController
{
    public static void PrintPlayer(Match match, int playerIdx, int indent) {
        var indentStr = new string('\t', indent);

        var player = match.GetPlayer(playerIdx);
        System.Console.WriteLine($"{indentStr}[{player.LogName}]");
        System.Console.WriteLine($"{indentStr}Loot plays: {player.LootPlays}");
        System.Console.WriteLine($"{indentStr}Coins: {player.Coins}");
    }
    public static void PrintStack(Match match) {
        System.Console.WriteLine("Stack: ");
        var count = match.Stack.Effects.Count;
        for (int i = 0; i < count; i++) {
            var effect = match.Stack.Effects[i];
            System.Console.WriteLine($"\t{i}: {effect}");
        }
    }

    public async Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint)
    {
        System.Console.WriteLine("(ChooseString)");
        System.Console.WriteLine(hint);
        System.Console.WriteLine("Options:");
        foreach (var option in options)
            System.Console.WriteLine("\t" + option);
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read player action in PromptAction")
        ;
        return result;
    }

    public async Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint)
    {
        System.Console.WriteLine("(ChoosePlayer)");
        System.Console.WriteLine(hint);
        System.Console.WriteLine("Options:");
        foreach (var option in options)
            System.Console.WriteLine($"\t{option} - ({match.GetPlayer(option).LogName})");
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read player choice in ChoosePlayer")
        ;
        return int.Parse(result);
    }

    public async Task<string> ChooseStackEffect(Match match, int playerIdx, List<string> options, string hint)
    {
        System.Console.WriteLine("(ChooseStackEffect)");
        System.Console.WriteLine(hint);
        System.Console.WriteLine("Options:");
        foreach (var option in options)
            System.Console.WriteLine($"\t{option}");
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read SID choice in ChooseStackEffect")
        ;
        return result;
    }

    public Task CleanUp(Match match, int playerIdx)
    {
        throw new NotImplementedException();
    }

    public async Task<string> PromptAction(Match match, int playerIdx, IEnumerable<string> options)
    {
        PrintStack(match);
        System.Console.WriteLine("-----------------");
        foreach (var player in match.Players)
            PrintPlayer(match, player.Idx, player.Idx == playerIdx ? 1 : 0);
        System.Console.WriteLine();
        System.Console.WriteLine("Options: ");
        foreach (var option in options)
            System.Console.WriteLine("\t" + option);
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read player action in PromptAction")
        ;
        return result;
    }

    public Task Setup(Match match, int playerIdx)
    {
        return Task.CompletedTask;
    }

    public Task Update(Match match, int playerIdx)
    {
        // TODO
        return Task.CompletedTask;
    }

    public Task<int> ChooseCardInHand(Match match, int playerIdx, List<int> options, string hint)
    {
        System.Console.WriteLine("(ChooseCardInHand)");
        System.Console.WriteLine(hint);
        System.Console.WriteLine("Options:");
        foreach (var option in options)
            System.Console.WriteLine($"\t{option} - {match.GetPlayer(playerIdx).Hand[option].Card.LogName}");
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read hand index choice in ChooseCardInHand")
        ;
        return Task.FromResult(int.Parse(result));
    }

    public Task<string> ChooseItem(Match match, int playerIdx, List<string> options, string hint)
    {
        System.Console.WriteLine("(ChooseItem)");
        System.Console.WriteLine(hint);
        System.Console.WriteLine("Options:");
        foreach (var option in options)
            System.Console.WriteLine($"\t{option} - {match.GetInPlayCard(option).LogName}");
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read item IPID choice in ChooseCardInHand")
        ;
        return Task.FromResult(result);   
    }
}

public class Program {
    private static MatchConfig ReadConfigYAML(string text) {
        var deserializer = new DeserializerBuilder()
            // .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<MatchConfig>(text);
    }

    public static async Task TcpMatch(MatchConfig config, int playerCount, int realPlayerCount) {
        var cm = new FileCardMaster();
        cm.Load("../cards/testing");

        var address = IPAddress.Any;
        int port = 9090;
        var endpoint = new IPEndPoint(address, port);
        TcpListener listener = new(endpoint);
        listener.Start();

        var match = new Match(config, 0, cm, File.ReadAllText("../core.lua")){
            Logger = LoggerFactory
                .Create(builder => builder.AddConsole())
                .CreateLogger("Match")
        };

        for (int i = 0; i < realPlayerCount; i++) {
            await AddTCPPlayer(listener, match);
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
        var c = new RandomPlayerController(0, 100);

        string name = "RandomPlayer";
        // TODO prompt player name with default name
        // TODO prompt player with character key

        var chKey = "guppy-v2";

        await match.AddPlayer(name, c, chKey);
    }

    public static async Task AddConsolePlayer(Match match) {
        var c = new ConsolePlayerController();

        string name = "ConsolePlayer";
        // TODO prompt player name with default name
        // TODO prompt player with character key

        var chKey = "guppy-v2";

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
            } catch (Exception e) {
                PrintException(e);
            }
        }
    }

    public static async Task Main(string[] args) {
        var config = ReadConfigYAML(
            File.ReadAllText("../configs/base.yaml")
        );

        await TcpLoop(config);
        return;

        await TcpMatch(config, 2, 0);
        return;

        var cm = new FileCardMaster();
        cm.Load("../cards/testing");

        var c1 = new ConsolePlayerController();
        var c2 = c1;

        var match = new Match(config, 0, cm, File.ReadAllText("../core.lua"));
        match.Logger = LoggerFactory
            .Create(builder => builder.AddConsole())
            .CreateLogger("Match");

        await match.AddPlayer("player1", c1);
        await match.AddPlayer("player2", c2);

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
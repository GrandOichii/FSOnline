using System.Reflection;
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
}

public class Program {
    private static MatchConfig ReadConfigYAML(string text) {
        var deserializer = new DeserializerBuilder()
            // .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<MatchConfig>(text);
    }

    public static async Task Main(string[] args) {
        var config = ReadConfigYAML(
            File.ReadAllText("../configs/base.yaml")
        );

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
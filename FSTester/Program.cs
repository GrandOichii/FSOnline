using FSCore.Matches;
using FSCore.Matches.Players.Controllers;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FSTester;

public class ConsolePlayerController : IPlayerController
{
    public Task CleanUp(Match match, int playerIdx)
    {
        throw new NotImplementedException();
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
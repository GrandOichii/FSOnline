using FSCore.Matches;
using FSCore.Matches.Players.Controllers;
using FSCore.Matches.Targets;

namespace FSTester;


public class ConsolePlayerController : IPlayerController
{
    public static void PrintPlayer(Match match, int playerIdx, int indent) {
        var indentStr = new string('\t', indent);

        var player = match.GetPlayer(playerIdx);
        System.Console.WriteLine($"{indentStr}[{player.LogName}]");
        System.Console.WriteLine($"{indentStr}Loot plays: {player.LootPlayed}");
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

    public Task<int> ChooseItemToPurchase(Match match, int playerIdx, List<int> options)
    {
        System.Console.WriteLine("(ChooseItemToPurchase)");
        System.Console.WriteLine("Options:");
        foreach (var option in options)
            System.Console.WriteLine($"\t{option} - " + (option >= 0 ? match.TreasureSlots[option].Card!.LogName : "Top of treasure deck"));
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read treasure slot in ChooseItemToPurchase")
        ;
        return Task.FromResult(int.Parse(result));   
    }

    public Task<int> ChooseMonsterToAttack(Match match, int playerIdx, List<int> options)
    {
        System.Console.WriteLine("(ChooseMonsterToAttack)");
        System.Console.WriteLine("Options:");
        foreach (var option in options)
            System.Console.WriteLine($"\t{option} - " + (option >= 0 ? match.MonsterSlots[option].Card!.LogName : "Top of monster deck"));
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read monster slot in ChooseMonsterToAttack")
        ;
        return Task.FromResult(int.Parse(result));   
    }

    public Task<(TargetType, string)> ChooseMonsterOrPlayer(Match match, int playerIdx, List<string> ipids, List<int> indicies, string hint)
    {
        System.Console.WriteLine("(ChooseMonsterOrPlayer)");
        System.Console.WriteLine("Monster options:");
        foreach (var option in ipids)
            System.Console.WriteLine(option);
        System.Console.WriteLine("Player options:");
        foreach (var option in indicies)
            System.Console.WriteLine(option);
            
        var result = Console.ReadLine()
            ?? throw new Exception("Failed to read monster/player in ChooseMonsterOrPlayer")
        ;
        return Task.FromResult((
            ipids.Contains(result) ? TargetType.ITEM : TargetType.PLAYER,
            result
        ));
    }
}

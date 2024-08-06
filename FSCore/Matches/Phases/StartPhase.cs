namespace FSCore.Matches.Phases;

/// <summary>
/// Turn start phase
/// </summary>
public class StartPhase : IPhase
{
    public string GetName() => "turn_start";

    public async Task PreEmit(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        await player.UntapAll();
        match.Stack.PriorityIdx = playerIdx;
    }

    public async Task PostEmit(Match match, int playerIdx)
    {
        // loot step
        var player = match.GetPlayer(playerIdx);
        await player.LootCards(
            match.Config.LootStepLootAmount, 
            LootReasons.LootPhase(match.LState)
        );

        if (match.Stack.Effects.Count > 0)
            await match.ResolveStack(true);
    }

}

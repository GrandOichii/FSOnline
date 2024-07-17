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
    }

    public async Task PostEmit(Match match, int playerIdx)
    {
        await match.ResolveStack();

        // loot step
        var player = match.GetPlayer(playerIdx);
        await player.LootCards(
            match.Config.LootStepLootAmount, 
            LootReasons.LootPhase(match.LState)
        );

        await match.ResolveStack();
    }

}

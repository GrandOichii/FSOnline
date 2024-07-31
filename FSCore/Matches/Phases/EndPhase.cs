namespace FSCore.Matches.Phases;

/// <summary>
/// Turn end phase
/// </summary>
public class EndPhase : IPhase
{
    public string GetName() => "turn_end";

    public async Task PostEmit(Match match, int playerIdx)
    {
        if (match.Stack.Effects.Count > 0)
            await match.ResolveStack(true);

        var player = match.GetPlayer(playerIdx);

        await player.DiscardToHandSize();
        await player.PromptToDiscardRoom();

        // pass the turn to the next player
        match.AdvanceCurrentPlayerIdx();

        foreach (var p in match.Players) {
            p.RemoveLootPlays();
            p.RemovePurchaseOpportunities();
            p.HealToMax();
            p.RollHistory.Clear();
            p.Stats.DamagePreventors.Clear();

            // reset trigger limits
            foreach (var card in p.GetInPlayCards()) {
                card.ResetTriggers();
            }
        }
        // TODO heal all monsters
        // TODO heal all rooms
        match.TEOTEffects.Clear();

        match.TurnEnded = false;
    }

    public Task PreEmit(Match match, int playerIdx)
    {
        // ? nothing?
        return Task.CompletedTask;
    }
}

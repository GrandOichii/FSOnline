namespace FSCore.Matches.Phases;

/// <summary>
/// Turn end phase
/// </summary>
public class EndPhase : IPhase
{
    public string GetName() => "turn_end";

    public async Task PostEmit(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);

        if (match.Stack.Effects.Count > 0)
            await match.ResolveStack(true);

        // "at the end of your turn" effects
        // TODO catch exceptions
        foreach (var effect in player.AtEndOfTurnEffects)
            effect.Call();


        await player.DiscardToHandSize();
        await player.PromptToDiscardRoom();

        // pass the turn to the next player
        match.AdvanceCurrentPlayerIdx();

        foreach (var p in match.Players) {
            p.RemovePurchaseOpportunities();
            p.RemoveAttackOpportunities();
            p.HealToMax();
            p.RollHistory.Clear();
            p.Stats.DamagePreventors.Clear();
            p.DeathPreventors.Clear();
            p.ResetLootPlays();

            // reset trigger limits
            foreach (var card in p.GetInPlayCards()) {
                card.ResetTriggers();
            }
        }

        foreach (var card in match.GetInPlayCards()) {
            await card.HealToMax();
        }

        match.TEOTEffects.Clear();
        match.DeadCards.Clear();
        player.AtEndOfTurnEffects.Clear();

        match.TurnEnded = false;
    }

    public Task PreEmit(Match match, int playerIdx)
    {
        // ? nothing?
        return Task.CompletedTask;
    }
}

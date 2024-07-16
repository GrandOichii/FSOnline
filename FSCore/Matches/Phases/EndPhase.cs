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

        await player.DiscardToHandSize();
        await player.PromptToDiscardRoom();

        // pass the turn to the next player
        match.AdvanceCurrentPlayerIdx();

        // TODO heal all players
        // TODO heal all monsters
        // TODO heal all rooms
        // TODO clear all "till end of turn" effects
    }

    public Task PreEmit(Match match, int playerIdx)
    {
        // ? nothing?
        return Task.CompletedTask;
    }
}

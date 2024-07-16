namespace FSCore.Matches.Phases;

/// <summary>
/// Action phase of turn
/// </summary>
public class ActionPhase : IPhase
{
    public string GetName() => "turn_action";

    public async Task PostEmit(Match match, int playerIdx)
    {
        // TODO
    }

    public async Task PreEmit(Match match, int playerIdx)
    {
        // TODO
    }
}

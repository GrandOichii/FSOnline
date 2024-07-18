namespace FSCore.Matches.Phases;

/// <summary>
/// Match setup phase, used for dummy purposes
/// </summary>
public class MatchSetupPhase : IPhase
{
    public string GetName() => "match_setup";

    public Task PostEmit(Match match, int playerIdx)
    {
        throw new MatchException("shouldn't attempt to call PostEmit of MatchSetupPhase");
    }

    public Task PreEmit(Match match, int playerIdx)
    {
        throw new MatchException("shouldn't attempt to call PreEmit of MatchSetupPhase");
    }
}

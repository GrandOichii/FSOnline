namespace FSCore.Matches.Players.Controllers;

public readonly struct MatchInfo {
    public MatchConfig Config { get; }
    public int PlayerIdx { get; }

    public MatchInfo(Match match, int playerI)
    {
        Config = match.Config;
        PlayerIdx = playerI;
    }
}
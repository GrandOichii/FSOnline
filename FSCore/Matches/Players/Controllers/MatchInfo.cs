namespace FSCore.Matches.Players.Controllers;

/// <summary>
/// Personalized match information
/// </summary>
public readonly struct MatchInfo {
    public MatchConfig Config { get; }
    public int PlayerIdx { get; }
    public int PlayerCount { get; }

    public MatchInfo(Match match, int playerI)
    {
        Config = match.Config;
        PlayerIdx = playerI;

        PlayerCount = match.Players.Count;
    }
}
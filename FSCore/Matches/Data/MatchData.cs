namespace FSCore.Matches.Data;

/// <summary>
/// Match data
/// </summary>
public class MatchData {
    /// <summary>
    /// Players' data
    /// </summary>
    public List<PlayerData> Players { get; }

    public MatchData(Match match) {
        
        Players = match.Players.Select(p => new PlayerData(p)).ToList();
        // TODO
    }
}
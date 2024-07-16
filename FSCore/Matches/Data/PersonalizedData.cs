namespace FSCore.Matches.Data;

/// <summary>
/// Match data with included personalized information
/// </summary>
public class PersonalizedData {
    /// <summary>
    /// Base match data
    /// </summary>
    public MatchData Match { get; }
    /// <summary>
    /// Player index
    /// </summary>
    public int PlayerIdx { get; }
    /// <summary>
    /// Request method
    /// </summary>
    public required string Request { get; set; }
    /// <summary>
    /// Request hint
    /// </summary>
    public required string Hint { get; set; }
    /// <summary>
    /// Request arguments, values are options
    /// </summary>
    public required Dictionary<string, object> Args { get; set; }

    public PersonalizedData(Match match, int playerIdx) {
        PlayerIdx = playerIdx;

        Match = new(match);
        // TODO
    }
}
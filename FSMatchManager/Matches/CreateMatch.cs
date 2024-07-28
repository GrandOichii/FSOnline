namespace FSMatchManager.Matches;

public struct CreateMatch {
    /// <summary>
    /// Match configuration
    /// </summary>
    public MatchConfig Config { get; set; }

    /// <summary>
    /// Optional match password
    /// </summary>
    public string Password { get; set; }
}
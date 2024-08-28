namespace FSManager.Matches;

public enum BotType {
    RANDOM = 0
}

public struct BotParams {
    public BotType Type { get; set; }
    public string Name { get; set; }
}

/// <summary>
/// Match creation parameters
/// </summary>
public struct CreateMatchParams {
    /// <summary>
    /// Match configuration
    /// </summary>
    public MatchConfig Config { get; set; }

    /// <summary>
    /// Optional match password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Parameters for bots
    /// </summary>
    public List<BotParams> Bots { get; set; }
}
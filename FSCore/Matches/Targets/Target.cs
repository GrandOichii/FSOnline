namespace FSCore.Matches.Targets;

/// <summary>
/// Target types
/// </summary>
public enum TargetType {
    PLAYER,
}

/// <summary>
/// Target for stack effect
/// </summary>
public readonly struct Target {
    /// <summary>
    /// Target type
    /// </summary>
    public TargetType Type { get; }
    /// <summary>
    /// Target value
    /// </summary>
    public string Value { get; }

    public Target(TargetType type, string value) {
        Type = type;
        Value = value;
    }
}
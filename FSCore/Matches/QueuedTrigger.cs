namespace FSCore.Matches;

/// <summary>
/// Queued trigger
/// </summary>
public readonly struct QueuedTrigger {
    /// <summary>
    /// Trigger
    /// </summary>
    public string Trigger { get; }
    /// <summary>
    /// Table of arguments
    /// </summary>
    public LuaTable Args { get; }

    public QueuedTrigger(string trigger, LuaTable args) {
        Trigger = trigger;
        Args = args;
    }
}
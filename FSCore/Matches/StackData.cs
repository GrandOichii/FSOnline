namespace FSCore.Matches;

public readonly struct StackData {
    /// <summary>
    /// Index of the player with priority
    /// </summary>
    public int PriorityIdx { get; }
    /// <summary>
    /// Stack effects
    /// </summary>
    public List<object> Effects { get; }

    public StackData(Stack stack) {
        PriorityIdx = stack.PriorityIdx;

        Effects = stack.Effects.Select(e => e.ToData()).ToList<object>();
    }
}
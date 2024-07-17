namespace FSCore.Matches.StackEffects;

public abstract class StackEffect {
    /// <summary>
    /// Parent match
    /// </summary>
    public Match Match { get; }
    /// <summary>
    /// Index of the owner
    /// </summary>
    public int OwnerIdx { get; }

    public StackEffect(Match match, int ownerIdx) {
        Match = match;
        OwnerIdx = ownerIdx;
    }

    /// <summary>
    /// Resolve the effect
    /// </summary>
    abstract public Task Resolve();
}


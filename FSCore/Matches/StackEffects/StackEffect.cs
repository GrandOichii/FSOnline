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
    /// <summary>
    /// Rolls results
    /// </summary>
    public List<int> Rolls { get; }

    public StackEffect(Match match, int ownerIdx) {
        Match = match;
        OwnerIdx = ownerIdx;
        
        Rolls = new();
    }

    /// <summary>
    /// Resolve the effect
    /// </summary>
    abstract public Task Resolve();
}


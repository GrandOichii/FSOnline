namespace FSCore.Matches.StackEffects;

public abstract class StackEffect {
    /// <summary>
    /// Stack ID
    /// </summary>
    public string SID { get; }
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
        
        SID = match.GenerateStackID();
        Rolls = new();
    }

    /// <summary>
    /// Resolve the effect
    /// </summary>
    abstract public Task Resolve();
    /// <summary>
    /// Get the data representation of the stack effect
    /// </summary>
    /// <returns>Stack effect data</returns>
    abstract public StackEffectData ToData();
}


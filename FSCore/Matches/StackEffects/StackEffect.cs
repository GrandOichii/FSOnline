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
    /// <summary>
    /// Effect targets
    /// </summary>
    public List<Target> Targets { get; }
    /// <summary>
    /// List of choices
    /// </summary>
    public Queue<int> Choices { get; }
    /// <summary>
    /// Shows whether the effect is cancelled
    /// </summary>
    public bool Cancelled { get; set; }

    public StackEffect(Match match, int ownerIdx) {
        Match = match;
        OwnerIdx = ownerIdx;
        
        SID = match.GenerateStackID();
        Rolls = [];
        Targets = [];
        Choices = [];
        Cancelled = false;
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

    public void AddChoice(int choice) {
        Choices.Enqueue(choice);
    }

    public int PopChoice() {
        return Choices.Dequeue();
    }
}


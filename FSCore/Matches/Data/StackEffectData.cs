namespace FSCore.Matches.Data;

public class StackEffectData {
    /// <summary>
    /// Stack ID
    /// </summary>
    public string SID { get; }
    /// <summary>
    /// Stack effect type
    /// </summary>
    public string Type { get; protected set; }
    /// <summary>
    /// Effect owner
    /// </summary>
    public int OwnerIdx { get; }
    public List<Target> Targets { get; }

    public StackEffectData(StackEffect effect) {
        SID = effect.SID;
        OwnerIdx = effect.OwnerIdx;
        Targets = effect.Targets;

        Type = "__base__";
    }
}
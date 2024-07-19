
namespace FSCore.Matches.Data;

public class RollStackEffectData : StackEffectData
{
    /// <summary>
    /// Stack ID of parent effect
    /// </summary>
    public string ParentSID { get; }
    /// <summary>
    /// Rolled value
    /// </summary>
    public int Value { get; }
    
    public RollStackEffectData(RollStackEffect effect)
        : base(effect)
    {
        ParentSID = effect.SID;
        Value = effect.Value;

        Type = "roll";
    }
}

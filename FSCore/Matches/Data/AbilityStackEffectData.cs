namespace FSCore.Matches.Data;

// TODO inherit from AbilityStackEffectData
public class AbilityStackEffectData : StackEffectData
{
    // TODO more
    /// <summary>
    /// Effect text
    /// </summary>
    public string EffectText { get; }
    /// <summary>
    /// In-play ID of parent card
    /// </summary>
    public string IPID { get; }

    public AbilityStackEffectData(StackEffect effect, string ipid, string effectText)
        : base(effect)
    {
        IPID = ipid;
        EffectText = effectText;

        Type = "ability_activation";
    }
}

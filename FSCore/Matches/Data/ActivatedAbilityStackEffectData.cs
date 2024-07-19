
namespace FSCore.Matches.Data;

public class ActivatedAbilityStackEffectData : StackEffectData
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

    public ActivatedAbilityStackEffectData(ActivatedAbilityStackEffect effect)
        : base(effect)
    {
        IPID = effect.Card.IPID;
        EffectText = effect.Ability.EffectText;

        Type = "ability_activation";
    }
}

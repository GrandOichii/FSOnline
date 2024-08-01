namespace FSCore.Matches.Data;

public class ActivatedAbilityStackEffectData : AbilityStackEffectData
{
    public ActivatedAbilityStackEffectData(ActivatedAbilityStackEffect effect)
        : base(effect, effect.Card.IPID, effect.Ability.EffectText)
    {
        Type = "ability_activation";
    }
}

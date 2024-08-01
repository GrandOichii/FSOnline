namespace FSCore.Matches.Data;

public class TriggeredAbilityStackEffectData : AbilityStackEffectData
{
    /// <summary>
    /// Trigger type
    /// </summary>
    public string TriggerType { get; }

    public TriggeredAbilityStackEffectData(TriggeredAbilityStackEffect effect)
        : base(effect, effect.Card.IPID, effect.Ability.EffectText)
    {
        TriggerType = effect.Trigger.Trigger;

        Type = "ability_trigger";
    }
}

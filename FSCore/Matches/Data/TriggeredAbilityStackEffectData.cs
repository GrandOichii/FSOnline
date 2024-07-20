namespace FSCore.Matches.Data;

// TODO inherit from AbilityStackEffectData
public class TriggeredAbilityStackEffectData : StackEffectData
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
    /// <summary>
    /// Trigger type
    /// </summary>
    public string TriggerType { get; }

    public TriggeredAbilityStackEffectData(TriggeredAbilityStackEffect effect)
        : base(effect)
    {
        IPID = effect.Card.IPID;
        EffectText = effect.Ability.EffectText;
        TriggerType = effect.Trigger.Trigger;

        Type = "ability_trigger";
    }
}

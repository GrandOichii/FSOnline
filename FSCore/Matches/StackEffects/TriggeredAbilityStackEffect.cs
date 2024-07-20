
namespace FSCore.Matches.StackEffects;

public class TriggeredAbilityStackEffect : StackEffect {
    /// <summary>
    /// Parent triggered ability
    /// </summary>
    public TriggeredAbility Ability { get; }
    /// <summary>
    /// Trigger causer
    /// </summary>
    public QueuedTrigger Trigger { get; }
    /// <summary>
    /// Card parent of trigger
    /// </summary>
    public InPlayMatchCard Card { get; }
    
    public TriggeredAbilityStackEffect(TriggeredAbility parent, InPlayMatchCard card, Player? owner, QueuedTrigger trigger)
        : base(card.Card.Match, owner is null ? -1 : owner.Idx)
    {
        Ability = parent;
        Trigger = trigger;
        Card = card;
    }

    public override async Task Resolve()
    {
        // TODO check fizzle
        Ability.ExecuteEffects(this, Trigger.Args);
    }

    // TODO
    public override StackEffectData ToData() => new TriggeredAbilityStackEffectData(this);
}
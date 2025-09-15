namespace FSCore.Matches.Abilities;

/// <summary>
/// Triggered ability wrapper
/// </summary>
public class TriggeredAbilityWrapper {
    public TriggeredAbility Ability { get; }

    public TriggeredAbilityWrapper(TriggeredAbility ability)
    {
        Ability = ability;
    }

    private async Task TryTrigger(InPlayMatchCard card, Player? owner, QueuedTrigger trigger) {
        if (Ability.TriggerLimit >= 0) {
            var amount = card.GetTriggerCount(Ability);
            if (amount >= Ability.TriggerLimit) return;
        }

        // lol
        if (Ability.Trigger != trigger.Trigger) return;

        var check = Ability.ExecuteCheck(card, owner, trigger.Args);
        if (!check) {
            return;
        }
        var match = card.Card.Match;

        var effect = new TriggeredAbilityStackEffect(Ability, card, owner, trigger);
        await match.PlaceOnStack(effect);

        var payed = Ability.PayCosts(card, owner ?? card.Card.Match.CurrentPlayer, effect, trigger.Args);
        
        if (!payed) {
            match.RemoveTopOfStack();
            return;
        }

        card.AddToTriggerCount(Ability);
    }

    public async Task TryTrigger(InPlayMatchCard card, QueuedTrigger trigger) {
        await TryTrigger(card, card.GetOwner(), trigger);
    }

}
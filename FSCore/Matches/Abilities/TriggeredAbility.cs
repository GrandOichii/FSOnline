namespace FSCore.Matches.Abilities;

public class TriggeredAbility : Ability {
    /// <summary>
    /// Ability trigger
    /// </summary>
    public string Trigger { get; }

    public TriggeredAbility(LuaTable table)
        : base(table)
    {
        // TODO
        Trigger = LuaUtility.TableGet<string>(table, "Trigger");
    }

    private async Task TryTrigger(InPlayMatchCard card, Player? owner, QueuedTrigger trigger) {
        // TODO catch errors
        // lol
        if (Trigger != trigger.Trigger) return;

        var check = ExecuteCheck(card, owner, trigger.Args);
        if (!check) {
            return;
        }
        var match = card.Card.Match;

        var effect = new TriggeredAbilityStackEffect(this, card, owner, trigger);
        await match.PlaceOnStack(effect);

        var payed = PayCosts(card, owner, effect, trigger.Args);
        if (!payed) {
            match.RemoveTopOfStack();

            return;
        }
    }

    public async Task TryTrigger(InPlayMatchCard card, QueuedTrigger trigger) {
        Player? owner = null;
        if (card is OwnedInPlayMatchCard ownedCard)
            owner = ownedCard.Owner;
        await TryTrigger(card, owner, trigger);
    }

}
namespace FSCore.Matches.Slots;

public class MonsterSlot : Slot
{
    // TODO add covered monsters

    public MonsterSlot(Deck source, int idx)
        : base("Monster", source, idx)
    {

    }

    // TODO
    public override SlotData GetData() => new SlotData(this);

    public override async Task Fill()
    {
        await base.Fill();

        if (Card is null) return;
        if (Card.Card.Template.Type == "Event" || Card.Card.Template.Type == "Curse") {
            var match = Card.Card.Match;
            var effect = new EventCardStackEffect(match, Card);
            await Card.Card.Match.PlaceOnStack(effect);

            var payed = PayCostsForEvent(Card, effect);
            if (!payed)
                throw new MatchException($"Player {match.CurrentPlayer.LogName} failed to pay costs for putting event card {Card.LogName} on stack");

            return;
        }

        if (Card.Card.Template.Type == "Curse") {
            // TODO
            return;
        }
    }

    public bool PayCostsForEvent(InPlayMatchCard card, StackEffect stackEffect) {
        var costs = card.Card.LootCosts;

        var player = card.Card.Match.CurrentPlayer;
        try {
            foreach (var cost in costs) {
                var returned = cost.Call(this, player, stackEffect);
                var payed = LuaUtility.GetReturnAsBool(returned);
                if (!payed)
                    return false;
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to execute cost functions for event card {card.LogName} by player {player.LogName}", e);
        }

        return true;
    }

    // public MonsterMatchCard GetCard() => (MonsterMatchCard)Card!;
}
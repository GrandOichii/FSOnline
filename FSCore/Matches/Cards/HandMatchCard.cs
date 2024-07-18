namespace FSCore.Matches.Cards;

/// <summary>
/// Hand wrapper for a Match Card
/// </summary>
public class HandMatchCard : IStateModifier {
    /// <summary>
    /// Original card
    /// </summary>
    public MatchCard Card { get; }
    /// <summary>
    /// Owner
    /// </summary>
    public Player Owner { get; }
    /// <summary>
    /// State
    /// </summary>
    public HandMatchCardState State { get; private set; }

    public HandMatchCard(MatchCard card, Player owner) {
        Card = card;
        Owner = owner;

        // Initial state
        State = new(this);
    }

    public void UpdateState()
    {
        State = new(this);
    }

    public void Modify(ModificationLayer layer)
    {
        // TODO
    }

    public bool PayCosts(StackEffect stackEffect) {
        var costs = Card.LootCosts;

        // TODO catch exceptions
        foreach (var cost in costs) {
            var returned = cost.Call(stackEffect);
            var payed = LuaUtility.GetReturnAsBool(returned);
            if (!payed)
                return false;
        }

        return true;
    }
}
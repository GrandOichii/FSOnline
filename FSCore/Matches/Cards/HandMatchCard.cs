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

    public bool CanPlay(Player player) {
        var checks = Card.LootChecks;

        // TODO merge these - on HandMatchCardState initialization fill PlayRestrictions with Card.LootChecks

        // TODO catch exceptions
        foreach (var restriction in State.PlayRestrictions) {
            var returned = restriction.Call(this, player);
            if (!LuaUtility.GetReturnAsBool(returned))
                return false;
        }

        // TODO catch exceptions
        foreach (var check in checks) {
            var returned = check.Call(this, player);
            var payed = LuaUtility.GetReturnAsBool(returned);
            if (!payed)
                return false;
        }

        return true;

    }

    public bool PayCosts(StackEffect stackEffect) {
        var costs = Card.LootCosts;

        // TODO catch exceptions
        foreach (var cost in costs) {
            var returned = cost.Call(this, stackEffect.Match.GetPlayer(stackEffect.OwnerIdx), stackEffect);
            var payed = LuaUtility.GetReturnAsBool(returned);
            if (!payed)
                return false;
        }

        return true;
    }
}
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
        var checks = new List<LuaFunction>(Card.LootChecks);
        checks.AddRange(State.PlayRestrictions);

        foreach (var check in checks) {
            try {
                var returned = check.Call(this, player);
                var payed = LuaUtility.GetReturnAsBool(returned);
                if (!payed)
                    return false;
            } catch (Exception e) {
                throw new MatchException($"Failed to execute loot play check/loot play restriction of card {Card.LogName} for player {player.Name}", e);
            }
        }

        return true;

    }

    public bool PayCosts(StackEffect stackEffect) {
        var costs = Card.LootCosts;

        var player = stackEffect.Match.GetPlayer(stackEffect.OwnerIdx);
        try {
            foreach (var cost in costs) {
                var returned = cost.Call(this, player, stackEffect);
                var payed = LuaUtility.GetReturnAsBool(returned);
                if (!payed)
                    return false;
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to execute cost functions for card {Card.LogName} by player {player.LogName}", e);
        }

        return true;
    }
}
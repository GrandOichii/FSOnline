namespace FSCore.Matches.Cards;

public class HandMatchCardState {
    /// <summary>
    /// Original hand card
    /// </summary>
    public HandMatchCard Original { get; }
    /// <summary>
    /// Indicies of players who can view the card
    /// </summary>
    public List<int> VisibleTo { get; }
    /// <summary>
    /// Loot cost
    /// </summary>
    public int LootCost { get; }
    /// <summary>
    /// Additional play restrictions
    /// </summary>
    public List<LuaFunction> PlayRestrictions { get; }

    public HandMatchCardState(HandMatchCard original) {
        Original = original;

        VisibleTo = [ Original.Owner.Idx ];
        LootCost = 1;
        PlayRestrictions = [];
    }
}
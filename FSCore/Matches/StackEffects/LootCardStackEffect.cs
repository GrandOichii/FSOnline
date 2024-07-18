
namespace FSCore.Matches.StackEffects;

public class LootCardStackEffect : StackEffect
{
    /// <summary>
    /// Loot card
    /// </summary>
    public MatchCard Card { get; }
    /// <summary>
    /// Indicates whether the loot card should go into loot discard after resolution
    /// </summary>
    public bool GoesToDiscard { get; set; }

    public LootCardStackEffect(Match match, int ownerIdx, MatchCard card)
        : base(match, ownerIdx)
    {
        Card = card;
        GoesToDiscard = true;
    }

    public override async Task Resolve()
    {
        Card.ExecuteCardEffects(this);

        // some cards put themselves into other zones !!! Trinkets
        if (GoesToDiscard)
            Match.PlaceIntoLootDiscard(Card);
    }
}

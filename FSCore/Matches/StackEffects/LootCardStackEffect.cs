
namespace FSCore.Matches.StackEffects;

public class LootCardStackEffect : StackEffect
{
    /// <summary>
    /// Loot card
    /// </summary>
    public MatchCard Card { get; }

    public LootCardStackEffect(Match match, int ownerIdx, MatchCard card)
        : base(match, ownerIdx)
    {
        Card = card;
    }

    public override async Task Resolve()
    {
        Card.ExecuteCardEffects(this);

        // TODO some cards put themselves into other zones
        Match.PlaceIntoLootDiscard(Card);
    }
}

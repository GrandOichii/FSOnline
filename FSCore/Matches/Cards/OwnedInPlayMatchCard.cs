namespace FSCore.Matches.Cards;

public class OwnedInPlayMatchCard : InPlayMatchCard
{
    /// <summary>
    /// Card owner
    /// </summary>
    public Player Owner { get; private set; }

    public OwnedInPlayMatchCard(MatchCard card, Player owner)
        : base(card)
    {
        Owner = owner;
    }

    public OwnedInPlayMatchCard(InPlayMatchCard original, Player owner)
        : base (original.Card)
    {
        Owner = owner;
        IPID = original.IPID;

        Tapped = original.Tapped;
        Counters = original.Counters;
    }

    public void SetOwner(Player owner) {
        Owner = owner;
    }

    public override bool CanBeActivatedBy(Player player)
    {
        if (Owner.Idx != player.Idx)
            return false;
        return base.CanBeActivatedBy(player);
    }
}
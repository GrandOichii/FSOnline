namespace FSCore.Matches.Cards;

public class OwnedInPlayMatchCard : InPlayMatchCard
{
    /// <summary>
    /// Card owner
    /// </summary>
    public Player Owner { get; }

    public OwnedInPlayMatchCard(MatchCard card, Player owner)
        : base(card)
    {
        Owner = owner;
    }

    public override bool CanBeActivatedBy(Player player)
    {
        if (Owner.Idx != player.Idx)
            return false;
        return base.CanBeActivatedBy(player);
    }
}
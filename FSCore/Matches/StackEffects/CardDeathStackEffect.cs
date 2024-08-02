namespace FSCore.Matches.StackEffects;

public class CardDeathStackEffect : StackEffect
{
    public StackEffect Source { get; }
    public InPlayMatchCard Card { get; }

    public CardDeathStackEffect(InPlayMatchCard card, StackEffect source)
        : base(card.Card.Match, -1)
    {
        Source = source;
        Card = card;
    }

    public override async Task<bool> Resolve()
    {
        await Card.ProcessDeath(Source);
        return true;
    }

    public override StackEffectData ToData() => new CardDeathStackEffectData(this);
}

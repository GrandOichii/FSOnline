

namespace FSCore.Matches.StackEffects;

public class EventCardStackEffect : StackEffect
{
    /// <summary>
    /// Loot card
    /// </summary>
    public InPlayMatchCard Card { get; }

    // TODO should owner idx be current player?
    public EventCardStackEffect(Match match, InPlayMatchCard card)
        : base(match, match.CurPlayerIdx)
    {
        Card = card;
    }

    public override async Task<bool> Resolve()
    {
        if (!Card.Card.ShouldFizzle(this)) {

            Card.Card.ExecuteCardEffects(this);

        } else {
            Match.LogDebug("Event card {LogName} fizzles", Card.LogName);
        }

        // some cards put themselves into other zones !!! Trinkets
        await Match.TryDiscardFromPlay(Card.IPID);

        return true;
    }

    public override StackEffectData ToData() => new EventCardStackEffectData(this);

    public string GetEffectText() => Card.Card.EffectText;
}


namespace FSCore.Matches.Data;

public class EventCardStackEffectData : StackEffectData
{
    /// <summary>
    /// Attached loot card data
    /// </summary>
    public InPlayCardData Card { get; }

    public EventCardStackEffectData(EventCardStackEffect effect)
        : base(effect)
    {
        Card = new(effect.Card);
        Type = "event";
    }
}

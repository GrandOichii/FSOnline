
namespace FSCore.Matches.Data;

public class EventCardStackEffectData : StackEffectData
{
    /// <summary>
    /// Attached loot card data
    /// </summary>
    public InPlayCardData Card { get; }
    public string EffectText { get; }

    public EventCardStackEffectData(EventCardStackEffect effect)
        : base(effect)
    {
        EffectText = effect.GetEffectText();
        Card = new(effect.Card);
        Type = "event";
    }
}

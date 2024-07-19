
namespace FSCore.Matches.Data;

public class LootCardStackEffectData : StackEffectData
{
    /// <summary>
    /// Attached loot card data
    /// </summary>
    public CardData Card { get; }

    public LootCardStackEffectData(LootCardStackEffect effect)
        : base(effect)
    {
        Card = new(effect.Card);
        Type = "loot_play";
    }
}

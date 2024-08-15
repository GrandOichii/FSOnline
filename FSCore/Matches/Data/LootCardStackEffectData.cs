
namespace FSCore.Matches.Data;

public class LootCardStackEffectData : StackEffectData
{
    /// <summary>
    /// Attached loot card data
    /// </summary>
    public CardData Card { get; }
    /// <summary>
    /// Effect text
    /// </summary>
    public string EffectText { get; }

    public LootCardStackEffectData(LootCardStackEffect effect)
        : base(effect)
    {
        EffectText = effect.GetEffectText();

        Card = new(effect.Card);
        Type = "loot_play";
    }
}

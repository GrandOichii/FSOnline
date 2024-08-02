namespace FSCore.Matches.Data;

public class CardDeathStackEffectData : StackEffectData
{
    public CardDeathStackEffectData(CardDeathStackEffect effect)
        : base(effect)
    {
        Type = "card_death";
    }
}

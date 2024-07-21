
namespace FSCore.Matches.Data;

public class DeclarePurchaseStackEffectData : StackEffectData
{
    public DeclarePurchaseStackEffectData(DeclarePurchaseStackEffect effect) : base(effect)
    {
        Type = "purchase_declaration";
    }
}

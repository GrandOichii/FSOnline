
namespace FSCore.Matches.Data;

public class DamageStackEffectData : StackEffectData
{
    public int Amount { get; }

    public DamageStackEffectData(DamageStackEffect effect)
        : base(effect)
    {
        Amount = effect.Amount;
        
        Type = "damage";
    }
}


namespace FSCore.Matches.Data;

public class DeclareAttackStackEffectData : StackEffectData
{
    public DeclareAttackStackEffectData(DeclareAttackStackEffect effect) : base(effect)
    {
        Type = "attack_declaration";
    }
}

namespace FSCore.Matches.Data;

public class AttackStackEffectData : StackEffectData
{
    public AttackStackEffectData(AttackStackEffect effect)
        : base(effect)
    {
        Type = "attack";
    }
}

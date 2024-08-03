namespace FSCore.Matches.Data;

public class RewardStackEffectData : StackEffectData
{
    public RewardStackEffectData(RewardStackEffect effect)
        : base(effect)
    {
        Type = "reward";
    }
}

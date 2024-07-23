namespace FSCore.Matches.Data;

public class PlayerDeathStackEffectData : StackEffectData
{
    public PlayerDeathStackEffectData(PlayerDeathStackEffect effect)
        : base(effect)
    {
        Type = "player_death";
    }
}

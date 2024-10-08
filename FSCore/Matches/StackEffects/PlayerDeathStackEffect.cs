namespace FSCore.Matches.StackEffects;

public class PlayerDeathStackEffect : StackEffect
{
    public StackEffect Source { get; }

    public PlayerDeathStackEffect(Player owner, StackEffect source)
        : base(owner.Match, owner.Idx)
    {
        Source = source;
    }

    public override async Task<bool> Resolve()
    {
        await Match.GetPlayer(OwnerIdx).ProcessDeath(Source);
        return true;
    }

    public override StackEffectData ToData() => new PlayerDeathStackEffectData(this);
}

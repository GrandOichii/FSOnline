namespace FSCore.Matches.StackEffects;

public class DamageStackEffect : StackEffect
{
    public int Amount { get; set; }
    public StackEffect DamageSource { get; }

    public DamageStackEffect(Match match, int ownerIdx, int amount, StackEffect damageSource)
        : base(match, ownerIdx)
    {
        Amount = amount;
        DamageSource = damageSource;
    }

    public DamageStackEffect(Match match, int ownerIdx, int amount, StackEffect damageSource, Player to)
        : this(match, ownerIdx, amount, damageSource)
    {
        Targets.Add(new(TargetType.PLAYER, to.Idx.ToString()));
    }

    public DamageStackEffect(Match match, int ownerIdx, int amount, StackEffect damageSource, InPlayMatchCard to)
        : this(match, ownerIdx, amount, damageSource)
    {
        Targets.Add(new(TargetType.ITEM, to.IPID));
    }

    public override async Task<bool> Resolve()
    {
        var target = Targets[0];

        switch (target.Type) {

        case TargetType.PLAYER:
            var player = Match.GetPlayer(int.Parse(target.Value));
            await player.ProcessDamage(Amount, DamageSource);
            return true;
        case TargetType.ITEM:
            var monster = Match.GetMonster(target.Value);
            await monster.ProcessDamage(Amount, DamageSource);
            return true;
        default:
            throw new MatchException($"Unexpected target type for DamageStackEffect: {target.Type} (value: {target.Value})");
        }
    }

    public override StackEffectData ToData() => new DamageStackEffectData(this);
}

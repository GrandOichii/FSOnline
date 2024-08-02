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
        Targets.Add(new(TargetType.MONSTER, to.IPID));
    }

    public override async Task<bool> Resolve()
    {
        // TODO monsters
        var target = Targets[0];

        if (target.Type == TargetType.PLAYER) {
            var player = Match.GetPlayer(int.Parse(target.Value));
            await player.ProcessDamage(Amount, DamageSource);
            return true;
        }

        if (target.Type == TargetType.MONSTER) {
            var monster = Match.GetMonster(target.Value);
            await monster.ProcessDamage(Amount, DamageSource);
            return true;
        }

        throw new MatchException($"Unexpected target type for DamageStackEffect: {target.Type} (value: {target.Value})");

        return true;
    }

    public override StackEffectData ToData() => new DamageStackEffectData(this);
}

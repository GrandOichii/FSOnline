namespace FSCore.Matches.StackEffects;

public class DamageStackEffect : StackEffect
{
    public int Amount { get; set; }
    public StackEffect DamageSource { get; }

    public DamageStackEffect(Player owner, int amount, StackEffect damageSource)
        : base(owner.Match, owner.Idx)
    {
        Amount = amount;
        DamageSource = damageSource;
    }

    public DamageStackEffect(Player owner, int amount, StackEffect damageSource, Player to)
        : this(owner, amount, damageSource)
    {
        Targets.Add(new(TargetType.PLAYER, to.Idx.ToString()));
    }

    public override async Task Resolve()
    {
        // TODO monsters
        var target = Targets[0];

        if (target.Type == TargetType.PLAYER) {
            var player = Match.GetPlayer(int.Parse(target.Value));
            await player.ProcessDamage(Amount, DamageSource);
            return;
        }

    }

    public override StackEffectData ToData() => new DamageStackEffectData(this);
}

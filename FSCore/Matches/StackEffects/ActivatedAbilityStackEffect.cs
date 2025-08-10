
namespace FSCore.Matches.StackEffects;

public class ActivatedAbilityStackEffect : StackEffect
{
    /// <summary>
    /// Parent ability
    /// </summary>
    public ActivatedAbility Ability { get; }
    /// <summary>
    /// Parent card
    /// </summary>
    public InPlayMatchCard Card { get; }

    public ActivatedAbilityStackEffect(ActivatedAbility ability, InPlayMatchCard card, Player owner)
        : base(owner.Match, owner.Idx)
    {
        Ability = ability;
        Card = card;

        Card = card;
    }

    public override Task<bool> Resolve()
    {
        if (Ability.ShouldFizzle(this)) {
            Match.LogDebug("Activated ability stack effect {StackID} of card {LogName} fizzles", SID, Card.LogName);
            // return true;
            return Task.FromResult(true);
        }
        Ability.ExecuteEffects(this);
        // return true;
        return Task.FromResult(true);
    }

    public override StackEffectData ToData() => new ActivatedAbilityStackEffectData(this);
}

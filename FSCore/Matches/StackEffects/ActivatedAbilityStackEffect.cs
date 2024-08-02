
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

    public override async Task<bool> Resolve()
    {
        if (Ability.ShouldFizzle(this)) {
            Match.LogInfo($"Activated ability stack effect {SID} of card {Card.LogName} fizzles");
            return true;
        }
        Ability.ExecuteEffects(this);
        return true;
    }

    public override StackEffectData ToData() => new ActivatedAbilityStackEffectData(this);
}


namespace FSCore.Matches.StackEffects;

public class RewardStackEffect : StackEffect
{
    /// <summary>
    /// Parent card
    /// </summary>
    public InPlayMatchCard Card { get; }
    public RewardAbility Reward { get; }

    public RewardStackEffect(InPlayMatchCard card, RewardAbility reward, int receiverIdx)
        : base(card.Card.Match, receiverIdx)
    {
        Card = card;
        Reward = reward;
    }

    public override Task<bool> Resolve()
    {
        // if (Ability.ShouldFizzle(this)) {
        //     Match.LogDebug("Reward ability stack effect {StackID} of card {LogName} fizzles", SID, Card.LogName);
        //     return true;
        // }
        Reward.ExecuteEffects(this);
        // return true;
        return Task.FromResult<bool>(true);
    }

    public override StackEffectData ToData() => new RewardStackEffectData(this);
}


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

    public override async Task<bool> Resolve()
    {
        // if (Ability.ShouldFizzle(this)) {
        //     Match.LogInfo($"Reward ability stack effect {SID} of card {Card.LogName} fizzles");
        //     return true;
        // }
        Reward.ExecuteEffects(this);
        return true;
    }

    public override StackEffectData ToData() => new RewardStackEffectData(this);
}

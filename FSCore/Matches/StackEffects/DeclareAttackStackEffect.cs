namespace FSCore.Matches.StackEffects;

public class DeclareAttackStackEffect : StackEffect {

    public DeclareAttackStackEffect(Match match, int ownerIdx)
        : base(match, ownerIdx)
    {
        
    }

    public override async Task<bool> Resolve()
    {
        // TODO
        var owner = Match.GetPlayer(OwnerIdx);

        var available = owner.AvailableToAttack();
        if (available.Count == 0) {
            Match.LogInfo($"Player {owner.LogName} can't attack anything");
            return true;
        }

        var slot = await owner.ChooseMonsterToAttack();

        // TODO pay costs for attacking

        // bool payed = owner.TryPayCoinsForSlot(slot);
        // if (!payed) {
        //     Match.LogInfo($"Player {owner.LogName} failed to pay the cost to purchase an item");
        //     return;
        // }

        if (slot == -1) {
            Match.LogInfo($"Player {owner.LogName} attacks the top card of the monster deck");
            // TODO
            return true;
        }

        Match.LogInfo($"Player {owner.LogName} attacks the monster in Monster slot {slot}");
        await owner.AttackMonsterInSlot(slot);
        return true;
    }

    public override StackEffectData ToData() => new DeclareAttackStackEffectData(this);
}
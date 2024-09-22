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
            Match.LogDebug("Player {LogName} can't attack anything", owner.LogName);
            return true;
        }

        var slot = await owner.ChooseMonsterToAttack();

        // TODO pay costs for attacking

        // bool payed = owner.TryPayCoinsForSlot(slot);
        // if (!payed) {
        //     Match.LogDebug("Player {LogName} failed to pay the cost to purchase an item", owner.LogName);
        //     return;
        // }

        if (slot == -1) {
            Match.LogDebug("Player {LogName} attacks the top card of the monster deck", owner.LogName);
            // TODO
            return true;
        }

        Match.LogDebug("Player {LogName} attacks the monster in Monster slot {SlotIdx}", owner.LogName, slot);
        await owner.AttackMonsterInSlot(slot);
        return true;
    }

    public override StackEffectData ToData() => new DeclareAttackStackEffectData(this);
}
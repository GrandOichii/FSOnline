namespace FSCore.Matches.StackEffects;

public class DeclarePurchaseStackEffect : StackEffect {

    public DeclarePurchaseStackEffect(Match match, int ownerIdx)
        : base(match, ownerIdx)
    {
        
    }

    public override async Task<bool> Resolve()
    {
        var owner = Match.GetPlayer(OwnerIdx);

        var available = owner.AvailableToPurchase();
        if (available.Count == 0) {
            Match.LogDebug("Player {LogName} can't purchase any items", owner.LogName);
            return true;
        }

        var slot = await owner.ChooseItemToPurchase();

        bool payed = owner.TryPayCoinsForSlot(slot);
        if (!payed) {
            Match.LogDebug("Player {LogName} failed to pay the cost to purchase an item", owner.LogName);
            return true;
        }

        if (slot == -1) {
            Match.LogDebug("Player {LogName} purchases the top card of the treasure deck", owner.LogName);
            await owner.GainTreasureRaw(1);
            return true;
        }

        Match.LogDebug("Player {LogName} purchases item in Shop slot {SlotIdx}", owner.LogName, slot);
        await owner.GainControl(Match.TreasureSlots[slot]);
        
        return true;
    }

    public override StackEffectData ToData() => new DeclarePurchaseStackEffectData(this);
}
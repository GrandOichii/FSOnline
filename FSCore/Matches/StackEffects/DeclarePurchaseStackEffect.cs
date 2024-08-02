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
            Match.LogInfo($"Player {owner.LogName} can't purchase any items");
            return true;
        }

        var slot = await owner.ChooseItemToPurchase();

        bool payed = owner.TryPayCoinsForSlot(slot);
        if (!payed) {
            Match.LogInfo($"Player {owner.LogName} failed to pay the cost to purchase an item");
            return true;
        }

        if (slot == -1) {
            Match.LogInfo($"Player {owner.LogName} purchases the top card of the treasure deck");
            await owner.GainTreasureRaw(1);
            return true;
        }

        Match.LogInfo($"Player {owner.LogName} purchases item in Shop slot {slot}");
        await owner.GainControl(Match.TreasureSlots[slot]);
        
        return true;
    }

    public override StackEffectData ToData() => new DeclarePurchaseStackEffectData(this);
}
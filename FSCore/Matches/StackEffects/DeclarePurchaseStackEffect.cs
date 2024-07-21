namespace FSCore.Matches.StackEffects;

public class DeclarePurchaseStackEffect : StackEffect {

    public DeclarePurchaseStackEffect(Match match, int ownerIdx)
        : base(match, ownerIdx)
    {
        
    }

    public override async Task Resolve()
    {
        // TODO
        var owner = Match.GetPlayer(OwnerIdx);

        // TODO this has to happen in Player
        var slot = -1;

        bool payed = owner.TryPayCoinsForSlot(slot);
        if (!payed) {
            Match.LogInfo($"Player {owner.LogName} failed to pay the cost to purchase an item");
            return;
        }

        if (slot == -1) {
            Match.LogInfo($"Player {owner.LogName} purchases the top card of the treasure deck");
            await owner.GainTreasureRaw(1);
            return;
        }
    }

    public override StackEffectData ToData() => new DeclarePurchaseStackEffectData(this);
}
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
        owner.PayCoins(Match.Config.PurchaseCost);
        await owner.GainTreasureRaw(1);
    }

    public override StackEffectData ToData() => new DeclarePurchaseStackEffectData(this);
}
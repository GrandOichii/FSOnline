namespace FSCore.Matches.Players;

public class PlayerState {
    public Player Original { get; }

    #region Modifiers

    public List<LuaFunction> CoinGainModifiers { get; }
    public List<LuaFunction> LootAmountModifiers { get; }
    public List<LuaFunction> PurchaseCostModifiers { get; }

    #endregion

    #region Replacement effects

    public List<LuaFunction> RollReplacementEffects { get; }

    #endregion

    public PlayerState(Player original)
    {
        Original = original;

        CoinGainModifiers = new();
        LootAmountModifiers = new();
        RollReplacementEffects = new();
        PurchaseCostModifiers = new();
    }
}
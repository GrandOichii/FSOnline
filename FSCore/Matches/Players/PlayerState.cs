namespace FSCore.Matches.Players;

public class PlayerState {
    public Player Original { get; }
    public int LootPlaysForTurn { get; set; }
    public int AdditionalSoulCount { get; set; }

    #region Modifiers

    public List<LuaFunction> CoinGainModifiers { get; }
    public List<LuaFunction> LootAmountModifiers { get; }
    public List<LuaFunction> PurchaseCostModifiers { get; }
    public List<LuaFunction> RollResultModifiers { get; }

    #endregion

    #region Replacement effects

    public List<LuaFunction> RollReplacementEffects { get; }

    #endregion

    #region Death
    
    public List<LuaFunction> DeathPenaltyReplacementEffects { get; }
    public List<LuaFunction> DeathPenaltyCoinLoseAmountModifiers { get; }
    public List<LuaFunction> DeathPenaltyLootDiscardAmountModifiers { get; }

    #endregion

    public PlayerState(Player original)
    {
        Original = original;
        LootPlaysForTurn = original.Match.Config.LootPlay;

        AdditionalSoulCount = 0;
        CoinGainModifiers = [];
        LootAmountModifiers = [];
        RollReplacementEffects = [];
        PurchaseCostModifiers = [];
        DeathPenaltyCoinLoseAmountModifiers = [];
        DeathPenaltyLootDiscardAmountModifiers = [];
        DeathPenaltyReplacementEffects = [];
        RollResultModifiers = [];
    }
}
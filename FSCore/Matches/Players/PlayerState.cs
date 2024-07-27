namespace FSCore.Matches.Players;

public class PlayerState {
    public Player Original { get; }
    public int LootPlaysForTurn { get; set; }
    public Stats Stats { get; }

    #region Modifiers

    public List<LuaFunction> CoinGainModifiers { get; }
    public List<LuaFunction> LootAmountModifiers { get; }
    public List<LuaFunction> PurchaseCostModifiers { get; }

    #endregion

    #region Replacement effects

    public List<LuaFunction> RollReplacementEffects { get; }

    #endregion

    #region Death

    public List<LuaFunction> DeathPenaltyCoinLoseAmountModifiers { get; }
    public List<LuaFunction> DeathPenaltyLootDiscardAmountModifiers { get; }

    #endregion

    public PlayerState(Player original)
    {
        Original = original;
        LootPlaysForTurn = original.Match.Config.LootPlay;

        CoinGainModifiers = [];
        LootAmountModifiers = [];
        RollReplacementEffects = [];
        PurchaseCostModifiers = [];
        DeathPenaltyCoinLoseAmountModifiers = [];
        DeathPenaltyLootDiscardAmountModifiers = [];

        Stats = new() {
            Attack = original.Character.GetTemplate().Attack,
            Health = original.Character.GetTemplate().Health,
        };
    }
}
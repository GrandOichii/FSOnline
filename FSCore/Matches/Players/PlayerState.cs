using FSCore.Matches.Players.Attacking;

namespace FSCore.Matches.Players;

public class PlayerState {
    public Player Original { get; }
    public int LootPlaysForTurn { get; set; }
    public bool UnlimitedLootPlays { get; set; }
    public int AdditionalSoulCount { get; set; }

    public Dictionary<string, IAttackOpportunity> AttackOpportunitiesFromCards { get; set;  }

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

        // TODO do the same for AttackOpportunities and PurchaseOpportunities
        LootPlaysForTurn = original.Match.CurPlayerIdx == original.Idx
            ? original.Match.Config.LootPlay 
            : 0;

        UnlimitedLootPlays = false;
        AdditionalSoulCount = 0;
        CoinGainModifiers = [];
        LootAmountModifiers = [];
        RollReplacementEffects = [];
        PurchaseCostModifiers = [];
        DeathPenaltyCoinLoseAmountModifiers = [];
        DeathPenaltyLootDiscardAmountModifiers = [];
        DeathPenaltyReplacementEffects = [];
        RollResultModifiers = [];
        AttackOpportunitiesFromCards = [];
    }
}
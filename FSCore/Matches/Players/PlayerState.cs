namespace FSCore.Matches.Players;

public class PlayerState {
    public Player Original { get; }

    #region Modifiers

    public List<LuaFunction> CoinGainModifiers { get; }

    #endregion

    #region Replacement effects

    public List<LuaFunction> RollReplacementEffects { get; }

    #endregion

    public PlayerState(Player original)
    {
        Original = original;

        CoinGainModifiers = new();
        RollReplacementEffects = new();
    }
}
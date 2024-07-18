namespace FSCore.Matches.Players;

public class PlayerState {
    public Player Original { get; }

    #region Modifiers

    public List<LuaFunction> CoinGainModifiers { get; }

    #endregion

    public PlayerState(Player original)
    {
        Original = original;

        CoinGainModifiers = new();
    }
}
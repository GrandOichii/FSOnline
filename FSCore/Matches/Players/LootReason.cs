namespace FSCore.Matches.Players;

public static class LootReasons {
    public static LuaTable LootPhase(Lua state) 
        => LuaUtility.CreateTable(state, new() {
            {"type", "loot_phase"},
        });

    public static LuaTable InitialDeal(Lua state)
        => LuaUtility.CreateTable(state, new() {
            {"type", "initial_deal"},
        });

    // TODO add others
}
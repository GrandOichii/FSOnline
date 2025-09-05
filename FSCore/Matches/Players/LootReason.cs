namespace FSCore.Matches.Players;

public static class LootReasons {
    public static LuaTable Empty(Lua state) 
        => LuaUtility.CreateTable(state, new() {
            {"type", "empty"},
        });

    public static LuaTable LootPhase(Lua state) 
        => LuaUtility.CreateTable(state, new() {
            {"type", "loot_phase"},
        });

    public static LuaTable InitialDeal(Lua state)
        => LuaUtility.CreateTable(state, new() {
            {"type", "initial_deal"},
        });

    public static LuaTable Effect(Lua state, StackEffect effect)
        => LuaUtility.CreateTable(state, new() {
            {"type", "effect"},
            {"stackEffect", effect},
        });
}
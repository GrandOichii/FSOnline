namespace FSCore.Matches.Abilities;

public class ActivatedAbility : Ability {
    /// <summary>
    /// Ability cost text
    /// </summary>
    public string CostText { get; }

    public ActivatedAbility(LuaTable table)
        : base(table)
    {
        CostText = LuaUtility.TableGet<string>(table, "CostText");
    }
}
namespace FSCore.Matches.Abilities;

public class TriggeredAbility : Ability {
    /// <summary>
    /// Ability trigger
    /// </summary>
    public string Trigger { get; }
    /// <summary>
    /// Number of times the ability can trigger per turn (-1 for no limit)
    /// </summary>
    public int TriggerLimit { get; private set; }

    public TriggeredAbility(LuaTable table)
        : base(table)
    {
        Trigger = LuaUtility.TableGet<string>(table, "Trigger");
        TriggerLimit = LuaUtility.GetInt(table, "TriggerLimit");
    }

}
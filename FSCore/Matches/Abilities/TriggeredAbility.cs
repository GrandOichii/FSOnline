namespace FSCore.Matches.Abilities;

/// <summary>
/// Triggered ability of a card
/// </summary>
public class TriggeredAbility : Ability
{
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
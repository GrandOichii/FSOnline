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

    public bool CanBeActivatedBy(InPlayMatchCard card, Player player) {
        if (!card.CanBeActivatedBy(player)) return false;
        // TODO other checks

        return ExecuteCheck(card, player);
    }
}
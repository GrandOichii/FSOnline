namespace FSCore.Matches.Abilities;

public class ActivatedAbilityWrapper {
    public ActivatedAbility Ability { get; }
    public List<LuaFunction> AdditionalChecks { get; }

    public ActivatedAbilityWrapper(ActivatedAbility ability)
    {
        Ability = ability;
        AdditionalChecks = new();
    }

    public bool CanBeActivatedBy(InPlayMatchCard card, Player player) {
        if (!card.CanBeActivatedBy(player)) return false;

        // TODO catch exceptions
        foreach (var check in AdditionalChecks) {
            var returned = check.Call(card, player);
            if (!LuaUtility.GetReturnAsBool(returned)) return false;
        }

        return Ability.ExecuteCheck(card, player);
    }

}
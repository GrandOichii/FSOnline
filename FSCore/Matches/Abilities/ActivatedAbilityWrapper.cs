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

        try {
            foreach (var check in AdditionalChecks) {
                var returned = check.Call(card, player);
                if (!LuaUtility.GetReturnAsBool(returned)) return false;
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to execute activated ability check for activation availability of card {card.LogName} for player {player.LogName}", e);
        }

        return Ability.ExecuteCheck(card, player);
    }

    public async Task Activate(InPlayMatchCard card, Player player, int myIdx) {
        var effect = new ActivatedAbilityStackEffect(Ability, card, player);
        await player.Match.PlaceOnStack(effect);

        var payed = Ability.PayCosts(card, player, effect);
        if (!payed) {
            player.Match.RemoveTopOfStack();
            player.Match.LogDebug("Player {PlayerLogName} decided not to pay activation costs for activated ability {AbilityIdx} of card {CardLogName}", player.LogName, myIdx, card.LogName);
            return;
        }

        await player.Match.Emit("item_activation", new() {
            { "Player", player },
            { "Item", card },
            { "Ability", this }
        });

        player.Match.LogDebug("Player {PlayerLogName} activated ability {AbilityIdx} of card {CardLogName}", player.LogName, myIdx, card.LogName);

    }

}
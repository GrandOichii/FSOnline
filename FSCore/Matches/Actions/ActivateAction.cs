
namespace FSCore.Matches.Actions;

public class ActivateAction : IAction
{
    public string ActionWord() => "a";

    public async Task Exec(Match match, int playerIdx, string[] args)
    {
        if (args.Length != 3) {
            match.PotentialError($"Expected args count of \"{ActionWord()}\" to be 3, but found {args.Length} (args: {string.Join(' ', args)})");
            return;
        }

        var player = match.GetPlayer(playerIdx);
        var card = match.GetInPlayCardOrDefault(args[1]);

        if (card is null) {
            match.PotentialError($"Player tried to activate card with IPID {args[1]}, which doesn't exist");
            return;
        }

        if (!int.TryParse(args[2], out int abilityIdx)) {
            match.PotentialError($"Invalid ability index for action \"{ActionWord()}\": {args[2]}");
            return;
        }

        var ability = card.GetActivatedAbility(abilityIdx);
        if (!ability.CanBeActivatedBy(card, player)) {
            throw new MatchException($"Activated ability activation check of card {card.LogName} failed during execution by player {player.LogName}");
        }

        var effect = new ActivatedAbilityStackEffect(ability.Ability, card, player);
        await match.PlaceOnStack(effect);

        var payed = ability.Ability.PayCosts(card, player, effect);
        if (!payed) {
            match.RemoveTopOfStack();
            match.LogInfo($"Player {player.LogName} decided not to pay activation costs for activated ability {abilityIdx} of card {card.LogName}");
            return;
        }

        await match.Emit("item_activation", new() {
            { "Item", card },
            { "Ability", ability }
        });

        match.LogInfo($"Player {player.LogName} activated ability {abilityIdx} of card {card.LogName}");
    }

    public IEnumerable<string> GetAvailable(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        var cards = player.GetInPlayCards();
        foreach (var card in cards) {
            var abilities = card.GetActivatedAbilities();
            for (int i = 0; i < abilities.Count; i++) {
                if (!abilities[i].CanBeActivatedBy(card, player)) continue;

                yield return $"{ActionWord()} {card.IPID} {i}";
            }
        }
    }
}
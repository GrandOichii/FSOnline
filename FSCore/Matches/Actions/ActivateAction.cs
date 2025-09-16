
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
        var cards = GetCards(player);
        var card = cards.FirstOrDefault(card => card.IPID == args[1]);

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

        await ability.Activate(card, player, abilityIdx);
    }

    public (IEnumerable<string> options, bool exclusive) GetAvailable(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);

        var cards = GetCards(player);

        List<string> result = [];

        foreach (var card in cards)
        {
            var abilities = card.GetActivatedAbilities();
            for (int i = 0; i < abilities.Count; i++)
            {
                if (!abilities[i].CanBeActivatedBy(card, player)) continue;

                result.Add($"{ActionWord()} {card.IPID} {i}");
            }
        }

        return (result, false);
    }

    private static List<InPlayMatchCard> GetCards(Player player) {
        // owned cards
        var result = new List<InPlayMatchCard>(
            player.GetInPlayCards()
        );

        // rooms
        if (player.Idx == player.Match.CurPlayerIdx)
            result.AddRange(
                player.Match.RoomSlots
                    .Where(s => s.Card is not null)
                    .Select(s => s.Card!)
            );
        return result;
    }
}
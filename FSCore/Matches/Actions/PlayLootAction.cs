namespace FSCore.Matches.Actions;

public class PlayLootAction : IAction
{
    public string ActionWord() => "p";

    public async Task Exec(Match match, int playerIdx, string[] args)
    {
        if (args.Length != 2) {
            match.PotentialError($"Expected args count of \"{ActionWord()}\" to be 2, but found {args.Length} (args: {string.Join(' ', args)})");
            return;
        }

        var cardID = args[1];
        var player = match.GetPlayer(playerIdx);
        var card = player.HandCardOrDefault(cardID);
        if (card is null) {
            match.PotentialError($"Player {player.LogName} tried to play loot card with ID {cardID} from hand, but the card was not found");
            return;
        }

        var played = await player.TryPlayCard(card);
        if (!played) {
            match.PotentialError($"Player {player.LogName} tried to play card {card.Card.LogName}, but failed to");
            return;
        }
    }

    public (IEnumerable<string> options, bool exclusive) GetAvailable(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);

        List<string> result = [];

        foreach (var card in player.Hand)
            if (player.CanPlay(card))
                result.Add($"{ActionWord()} {card.Card.ID}");

        return (result, false);
    }
}

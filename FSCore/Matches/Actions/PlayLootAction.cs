namespace FSCore.Matches.Actions;

public class PlayLootAction : IAction
{
    public string ActionWord() => "p";

    public async Task Exec(Match match, int playerIdx, string[] args)
    {
        if (args.Length != 2) {
            match.PotentialError($"Invalid amount of arguments for action \"{ActionWord()}\": {args.Length} (args: {string.Join(' ', args)})");
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
            match.PotentialError($"Player {player.LogName} tried to play card {card.Card.LogName}, bu failed to");
            return;
        }
    }

    public IEnumerable<string> GetAvailable(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);

        foreach (var card in player.Hand)
            if (player.CanPlay(card))
                yield return $"{ActionWord()} {card.Card.ID}";
    }
}

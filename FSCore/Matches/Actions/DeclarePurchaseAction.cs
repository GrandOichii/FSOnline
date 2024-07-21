
namespace FSCore.Matches.Actions;

public class DeclarePurchaseAction : IAction
{
    public string ActionWord() => "dp";

    public async Task Exec(Match match, int playerIdx, string[] args)
    {
        if (args.Length != 1) {
            match.PotentialError($"Expected args count of \"{ActionWord()}\" to be 1, but found {args.Length} (args: {string.Join(' ', args)})");
            return;
        }

        // TODO should be done in Player
        var player = match.GetPlayer(playerIdx);

        // TODO is this supposed to be here or during resolution
        player.PurchaseOpportunities--;

        var effect = new DeclarePurchaseStackEffect(match, playerIdx);

        match.LogInfo($"Player {player.LogName} declares a purchase");
        await match.PlaceOnStack(effect);

        // TODO trigger
    }

    public IEnumerable<string> GetAvailable(Match match, int playerIdx)
    {
        if (match.CurPlayerIdx != playerIdx) yield break;
        if (match.Stack.Effects.Count > 0) yield break;

        // TODO get all available purchases and check their length
        var player = match.GetPlayer(playerIdx);

        if (player.PurchaseOpportunities == 0) yield break;

        // TODO check purchase cost of each individual shop item/top of treasure deck
        if (player.AvailableToPurchase().Count == 0) yield break;

        yield return ActionWord();        
    }
}

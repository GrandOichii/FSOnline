
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

        var player = match.GetPlayer(playerIdx);
        await player.DeclarePurchase();
    }

    public (IEnumerable<string> options, bool exclusive) GetAvailable(Match match, int playerIdx)
    {
        if (match.CurPlayerIdx != playerIdx) return ([], false);
        if (match.Stack.Effects.Count > 0) return ([], false);

        var player = match.GetPlayer(playerIdx);

        if (player.PurchaseOpportunities == 0) return ([], false);

        if (player.AvailableToPurchase().Count == 0) return ([], false);

        return ([ActionWord()], false);        
    }
}

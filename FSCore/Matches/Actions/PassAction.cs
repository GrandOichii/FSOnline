
namespace FSCore.Matches.Actions;

public class PassAction : IAction
{
    public string ActionWord() => "p";

    public async Task Exec(Match match, int playerIdx, string[] args)
    {
        if (args.Length != 1) {
            match.PotentialError($"Expected args count of PassAction to be 1, but found {args.Length} (args: {string.Join(' ', args)})");
            return;
        }

        var player = match.GetPlayer(playerIdx);
        await match.ProcessPass(player);
    }

    public IEnumerable<string> GetAvailable(Match match, int playerIdx)
    {
        // TODO check if current player can end the turn

        yield return ActionWord();
    }
}

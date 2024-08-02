
namespace FSCore.Matches.Actions;

public class DeclareAttackAction : IAction
{
    public string ActionWord() => "da";

    public async Task Exec(Match match, int playerIdx, string[] args)
    {
        if (args.Length != 1) {
            match.PotentialError($"Expected args count of \"{ActionWord()}\" to be 1, but found {args.Length} (args: {string.Join(' ', args)})");
            return;
        }

        var player = match.GetPlayer(playerIdx);
        await player.DeclareAttack();
    }

    public IEnumerable<string> GetAvailable(Match match, int playerIdx)
    {
        if (match.CurPlayerIdx != playerIdx) yield break;
        if (match.Stack.Effects.Count > 0) yield break;

        var player = match.GetPlayer(playerIdx);

        if (player.AttackOpportunities == 0) yield break;

        if (player.AvailableToAttack().Count == 0) yield break;

        yield return ActionWord();        
    }
}

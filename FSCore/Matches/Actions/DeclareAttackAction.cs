
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

    public (IEnumerable<string> options, bool exclusive) GetAvailable(Match match, int playerIdx)
    {
        if (match.CurPlayerIdx != playerIdx) return ([], false);
        if (match.Stack.Effects.Count > 0) return ([], false);

        var player = match.GetPlayer(playerIdx);

        if (player.AvailableToAttack().Count == 0) return ([], false);

        return ([ActionWord()], player.AttackOpportunities.HasToAttack());        
    }
}

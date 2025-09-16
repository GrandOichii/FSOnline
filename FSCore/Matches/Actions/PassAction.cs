using FSCore.Matches.Players.Attacking;

namespace FSCore.Matches.Actions;

public class PassAction : IAction
{
    public string ActionWord() => "pass";

    public async Task Exec(Match match, int playerIdx, string[] args)
    {
        if (args.Length != 1)
        {
            match.PotentialError($"Expected args count of \"{ActionWord()}\" to be 1, but found {args.Length} (args: {string.Join(' ', args)})");
            return;
        }

        var player = match.GetPlayer(playerIdx);

        match.LogDebug("Player {LogName} passes", player.LogName);
        await match.ProcessPass(player);
    }

    public (IEnumerable<string> options, bool exclusive) GetAvailable(Match match, int playerIdx)
    {
        return ([ActionWord()], false);
    }
}

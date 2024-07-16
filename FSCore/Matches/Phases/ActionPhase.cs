using System.Text.RegularExpressions;

namespace FSCore.Matches.Phases;

/// <summary>
/// Thrown when encounter an unknown action from a player
/// </summary>
[Serializable]
public class UnknownActionException : MatchException
{
    public UnknownActionException() : base() { }

    public UnknownActionException(string message) : base(message) { }
}


/// <summary>
/// Action phase of turn
/// </summary>
public class ActionPhase : IPhase
{
    /// <summary>
    /// Setup-list of actions a player can make
    /// </summary>
    private static readonly List<IAction> ACTIONS = new() {
        new PassAction(),
    };

    /// <summary>
    /// Player action index
    /// </summary>
    private static readonly Dictionary<string, IAction> ACTION_MAP = new(){};

    static ActionPhase() {
        foreach (var action in ACTIONS) {
            ACTION_MAP.Add(action.ActionWord(), action);
        }
    }

    public string GetName() => "turn_action";

    public async Task PostEmit(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        player.AddLootPlayForTurn();
        
        string action;

        while (true) {
            await match.ReloadState();
            if (!match.Active) return;

            action = await PromptAction(player);

            var words = action.Split(" ");
            var actionWord = words[0];
            if (!ACTION_MAP.ContainsKey(actionWord)) {
                if (match.Config.StrictMode) continue;

                throw new UnknownActionException($"Unknown action from player {player.LogName}: {action}");
            }

            await ACTION_MAP[actionWord].Exec(match, playerIdx, words);

            if (!match.Active || match.TurnEnded) return;

        }
    }

    private async Task<string> PromptAction(Player player) {
        return "";
    }

    public Task PreEmit(Match match, int playerIdx)
    {
        return Task.CompletedTask;
    }
}

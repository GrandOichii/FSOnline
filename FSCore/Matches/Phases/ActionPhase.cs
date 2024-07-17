using System.Runtime.InteropServices;
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
        new PlayLootAction(),
        new ActivateAction(),
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
        var current = match.GetPlayer(playerIdx);
        current.AddLootPlayForTurn();
        
        string action;

        while (true) {
            await match.ReloadState();
            if (!match.Active) return;

            var player = match.GetPriorityPlayer();
            action = await PromptAction(player);

            var words = action.Split(" ");
            var actionWord = words[0];
            if (!ACTION_MAP.ContainsKey(actionWord)) {
                if (!match.Config.StrictMode) {
                    match.LogWarning($"Unknown action word from player {player.LogName}: {actionWord}");
                    continue;
                }

                throw new UnknownActionException($"Unknown action from player {player.LogName}: {action}");
            }

            await ACTION_MAP[actionWord].Exec(match, player.Idx, words);
            await match.ResolveStack();

            if (!match.Active || match.TurnEnded) return;
        }
    }

    /// <summary>
    /// Prompts the player to perform an action
    /// </summary>
    /// <param name="player">Player</param>
    /// <returns>Action string</returns>
    /// <exception cref="MatchException"></exception>
    private static async Task<string> PromptAction(Player player) {
        var options = new List<string>();
        foreach (var action in ACTION_MAP.Values)
            options.AddRange(action.GetAvailable(player.Match, player.Idx));

        
        if (options.Count == 0) {
            throw new MatchException($"Player {player.LogName} doens't have any available actions");
        }
        return await player.Controller.PromptAction(player.Match, player.Idx, options);
    }

    public Task PreEmit(Match match, int playerIdx)
    {
        return Task.CompletedTask;
    }
}

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
    public string GetName() => "turn_action";

    public async Task PostEmit(Match match, int playerIdx)
    {
        var current = match.GetPlayer(playerIdx);
        current.AddLootPlayForTurn();
        current.AddPurchaseOpportunitiesForTurn();
        current.AddAttackOpportunitiesForTurn();
        // TODO attack opportunities
        
        await match.ResolveStack();
    }

    public Task PreEmit(Match match, int playerIdx)
    {
        return Task.CompletedTask;
    }
}

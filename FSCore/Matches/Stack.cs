namespace FSCore.Matches;

/// <summary>
/// Effect stack
/// </summary>
public class Stack {
    public static readonly int NO_PRIORITY_IDX = -2;
    /// <summary>
    /// Parent match
    /// </summary>
    public Match Match { get; }
    /// <summary>
    /// Stack effects
    /// </summary>
    public List<StackEffect> Effects { get; }
    /// <summary>
    /// Index of the player with priority
    /// </summary>
    public int PriorityIdx { get; set; }
    /// <summary>
    /// Queued triggers
    /// </summary>
    public Queue<QueuedTrigger> QueuedTriggers { get; }

    /// <summary>
    /// Top effect
    /// </summary>
    public StackEffect Top => Effects.Last();

    /// <summary>
    /// True, if stack can pass priority to the next player
    /// </summary>
    public bool CanPassPriority {
        get {
            // TODO add more
            return PriorityIdx != NO_PRIORITY_IDX && Effects.Count > 0;
        }
    }

    public Stack(Match match) {
        Match = match;

        Effects = new();
        PriorityIdx = NO_PRIORITY_IDX;
        QueuedTriggers = new();
    }

    /// <summary>
    /// Process a pass action a player
    /// </summary>
    /// <param name="player">Player, performing the passa action</param>
    /// <returns>True if the stack is empty/can't pass priority</returns>
    public async Task<bool> ProcessPass(Player player) {
        if (!CanPassPriority) return true;
        
        // advance priority
        PriorityIdx = Match.NextInTurnOrder(PriorityIdx);
        Match.LogInfo($"New priority player index: {PriorityIdx}");

        var top = Top;
        var topOwner = top.OwnerIdx > 0
            ? top.OwnerIdx
            : Match.CurPlayerIdx;
        if (PriorityIdx == topOwner) {
            // if (top.OwnerIdx != player.Idx) {
            //     throw new MatchException($"Unexpected scenario: resolving top of stack effect, owned by idx {top.OwnerIdx}, by player {player.LogName}");
            // }
            await ResolveTop();
        }

        return false;
    }

    /// <summary>
    /// Resolve the top effect and remove from stack
    /// </summary>
    public async Task ResolveTop() {
        Match.LogInfo("Resolving top of stack");
        var top = Top;

        await top.Resolve();

        Effects.Remove(top);
        PriorityIdx = Match.CurPlayerIdx;
        if (Effects.Count == 0) {
            Match.LogInfo("Stack is now empty");
            PriorityIdx = NO_PRIORITY_IDX;
        }
    }

    /// <summary>
    /// Add a new effect to the top of the stack
    /// </summary>
    /// <param name="effect">New effect</param>
    public void AddEffect(StackEffect effect) {
        Effects.Add(effect);
        PriorityIdx = effect.OwnerIdx;

        Match.LogInfo($"A new effect was added to the top of stack");
        // TODO add update
    }

    public async Task Resolve(bool breakIfPass) {
        while (true) {
            await Match.ReloadState();
            if (!Match.Active) return;

            var player = Match.GetPriorityPlayer();
            await player.PerformAction();
            if (Effects.Count > 0) continue;
            if (breakIfPass) return;

            if (!Match.Active || Match.TurnEnded) return;
        }
    }


    public void QueueTrigger(QueuedTrigger trigger) {
        QueuedTriggers.Enqueue(trigger);
    }
    
}
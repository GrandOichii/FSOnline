

namespace FSCore.Matches.StackEffects;

public class RollStackEffect : StackEffect
{
    /// <summary>
    /// Roll value
    /// </summary>
    public int Value { get; private set; }
    /// <summary>
    /// Shows whether the roll is an attack roll
    /// </summary>
    public bool IsAttackRoll { get; }

    public RollStackEffect(Match match, StackEffect parent, bool isAttackRoll)
        : base(match, parent.OwnerIdx > 0 ? parent.OwnerIdx : match.CurPlayerIdx)
    {
        Parent = parent;
        IsAttackRoll = isAttackRoll;

        Roll();
    }

    /// <summary>
    /// Sets the roll value to a random value between 1 and 6
    /// </summary>
    private void Roll() {
        Value = Match.Rng.Next(1, 7);
        // Value = 1;

        Match.LogInfo($"Player {Match.GetPlayer(Parent!.OwnerIdx).LogName} rolled a {Value}");

        // TODO request player to order replacement effects
        var owner = Match.GetPlayer(OwnerIdx);
        foreach (var repEffect in owner.State.RollReplacementEffects) {
            var returned = repEffect.Call(this);
            var stop = !LuaUtility.GetReturnAsBool(returned);
            if (stop) break;
        }
    }

    public void SetValue(int value) {
        Value = Math.Clamp(value, 1, 6);

        Match.LogInfo($"Roll value of roll {SID} is set to {Value}");
    }

    public override async Task<bool> Resolve()
    {
        var value = Match.GetPlayer(OwnerIdx).CalculateRollResult(this);
        // TODO? completely fizzle the roll?
        if (!Parent!.Cancelled) {
            Parent.Rolls.Add(value);
        }

        var player = Match.GetPlayer(OwnerIdx);
        player.RollHistory.Add(value);

        await Match.Emit("roll", new() {
            { "Value", value },
            { "Player", player },
        });

        return true;
    }

    public override StackEffectData ToData() => new RollStackEffectData(this);

    /// <summary>
    /// Rerolls the value
    /// </summary>
    public void Reroll() {
        Match.LogInfo($"Roll stack effect {SID} reroll request");

        Roll();
    }

    public void Modify(int mod) {
        var prev = Value;
        Value += mod;

        if (Value < 1 || Value > 6) 
            throw new MatchException($"Roll modification led to roll value being set to {Value} (original: {prev}, mod: {mod})");

        Match.LogInfo($"Modified roll value {prev} -> {Value}");
    }
}

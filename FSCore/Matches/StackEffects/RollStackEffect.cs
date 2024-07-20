

namespace FSCore.Matches.StackEffects;

public class RollStackEffect : StackEffect
{
    /// <summary>
    /// Parent stack effect
    /// </summary>
    public StackEffect Parent { get; }
    /// <summary>
    /// Roll value
    /// </summary>
    public int Value { get; private set; }

    public RollStackEffect(Match match, StackEffect parent)
        : base(match, parent.OwnerIdx)
    {
        Parent = parent;

        RandomizeValue();
    }

    /// <summary>
    /// Sets the roll value to a random value between 1 and 6
    /// </summary>
    private void RandomizeValue() {
        Value = Match.Rng.Next(1, 7);

        Match.LogInfo($"Player {Match.GetPlayer(Parent.OwnerIdx).LogName} rolled a {Value}");
    }

    public void SetValue(int value) {
        Value = value;

        Match.LogInfo($"Roll value of roll {SID} is set to {Value}");
    }

    public override async Task Resolve()
    {
        Parent.Rolls.Add(Value);

        // TODO add trigger
    }

    public override StackEffectData ToData() => new RollStackEffectData(this);

    /// <summary>
    /// Rerolls the value
    /// </summary>
    public void Reroll() {
        Match.LogInfo($"Roll stack effect {SID} reroll request");

        RandomizeValue();
    }

    public void Modify(int mod) {
        var prev = Value;
        Value += mod;

        if (Value < 1 || Value > 6) 
            throw new MatchException($"Roll modification led to roll value being set to {Value} (original: {prev}, mod: {mod})");

        Match.LogInfo($"Modified roll value {prev} -> {Value}");
    }
}

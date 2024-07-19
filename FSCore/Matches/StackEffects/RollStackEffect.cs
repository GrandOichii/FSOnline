

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

        SetValue();
    }

    /// <summary>
    /// Sets the roll value to a random value between 1 and 6
    /// </summary>
    private void SetValue() {
        Value = Match.Rng.Next(1, 7);

        Match.LogInfo($"Player {Match.GetPlayer(Parent.OwnerIdx).LogName} rolled a {Value}");
    }

    public override async Task Resolve()
    {
        Parent.Rolls.Add(Value);
    }

    public override StackEffectData ToData() => new RollStackEffectData(this);

    /// <summary>
    /// Rerolls the value
    /// </summary>
    public void Reroll() {
        Match.LogInfo($"Roll stack effect {SID} reroll request");

        SetValue();
    }
}

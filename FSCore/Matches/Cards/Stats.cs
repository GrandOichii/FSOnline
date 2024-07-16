namespace FSCore.Matches.Cards;

/// <summary>
/// Game object stats
/// </summary>
public class Stats {
    public int Attack { get; }
    public int Health { get; }
    public int? Evasion { get; }

    public bool CanBeAttacked() {
        if (Evasion is null) return false;
        // TODO more

        return true;
    }
}
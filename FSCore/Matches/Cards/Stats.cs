namespace FSCore.Matches.Cards;

/// <summary>
/// Game object stats
/// </summary>
public class Stats {
    public int Attack { get; set; }
    public int Health { get; set; }
    public int? Evasion { get; set; }

    // public bool CanBeAttacked() {
    //     if (Evasion is null) return false;
    //     // TODO more

    //     return true;
    // }
}
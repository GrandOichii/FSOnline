namespace FSCore.Matches.Data;

public readonly struct StatsData(Stats stats) {
    /// <summary>
    /// Health
    /// </summary>
    public int Health { get; } = stats.State.Health - stats.Damage;
    /// <summary>
    /// Attack
    /// </summary>
    public int Attack { get; } = stats.State.Attack;
    /// <summary>
    /// Amount of damage that can be prevented
    /// </summary>
    public int PreventableDamage { get; } = stats.DamagePreventors.Count;
    public int? Evasion { get; } = stats.State.Evasion;
}
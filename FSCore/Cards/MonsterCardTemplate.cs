namespace FSCore.Cards;

/// <summary>
/// Monster card template, describes a monster's stats as well as rewards
/// </summary>
public class MonsterCardTemplate : CardTemplate {
    /// <summary>
    /// Rewards text
    /// </summary>
    public required string RewardsText { get; set; }
}
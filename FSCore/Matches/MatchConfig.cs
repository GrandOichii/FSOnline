namespace FSCore.Matches;

/// <summary>
/// Match configuration
/// </summary>
public struct MatchConfig {
    /// <summary>
    /// Number of players
    /// </summary>
    public required int PlayerCount { get; set; }
    /// <summary>
    /// Total coin pool, set to -1 for unlimited coin count
    /// </summary>
    public required int CoinPool { get; set; }
    /// <summary>
    /// Initial amount of shop slots
    /// </summary>
    public required int InitialTreasureSlots { get; set; }
    /// <summary>
    /// Initial amount of monster slots
    /// </summary>
    public required int InitialMonsterSlots { get; set; }
    /// <summary>
    /// Initial amount of room slots
    /// </summary>
    public required int InitialRoomSlots { get; set; }
    /// <summary>
    /// Amount of bonus souls used
    /// </summary>
    public required int BonusSoulCount { get; set; }
    /// <summary>
    /// Amount of loot cards initially dealt to each player
    /// </summary>
    public required int InitialDealLoot { get; set; }
    /// <summary>
    /// Amount of coins initially dealt to each player
    /// </summary>
    public required int InitialDealCoins { get; set; }
    /// <summary>
    /// Amount of souls required to win
    /// </summary>
    public required int SoulsToWin { get; set; }
    /// <summary>
    /// Include cards labeled as 3+ event in a 2-player match
    /// </summary>
    public required bool ForceInclude3PlusCards { get; set; }
    /// <summary>
    /// Is the first player chosen randomly (otherwise the first player will be the first one that was added to the match)
    /// </summary>
    public required bool RandomFirstPlayer { get; set; }
}
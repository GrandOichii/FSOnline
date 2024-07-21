namespace FSCore.Matches;

/// <summary>
/// Match configuration
/// </summary>
public struct MatchConfig {
    /// <summary>
    /// Maximum amount of players allowed to join the match
    /// </summary>
    public required int MaxPlayerCount { get; set; }
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
    /// <summary>
    /// Maximum amount of loot cards a player can hold by the end of their turn
    /// </summary>
    public required int MaxHandSize { get; set; }
    /// <summary>
    /// Amount of cards to be drawn during the Loot Step
    /// </summary>
    public required int LootStepLootAmount { get; set; }
    /// <summary>
    /// Loot deck card keys and their amounts
    /// </summary>
    public required Dictionary<string, int> LootCards { get; set; }
    /// <summary>
    /// Keys of character cards
    /// </summary>
    public required List<string> Characters { get; set; } // TODO not yet used
    /// <summary>
    /// Keys of starting items
    /// </summary>
    public required List<string> StartingItems { get; set; }
    /// <summary>
    /// Keys of treasures
    /// </summary>
    public required List<string> Treasures { get; set; }
    /// <summary>
    /// Shows whether the match will be more prone to crashing after a player provides unknown actions, tries to pass a turn when can't, e.t.c.
    /// </summary>
    public required bool StrictMode { get; set; }
    /// <summary>
    /// Default maximum amount of shop items a player can purchase per turn
    /// </summary>
    public required int PurchaseCountDefault { get; set; }
    /// <summary>
    /// Default maximum amount of times a player can attack per turn
    /// </summary>
    public required int AttackCountDefault { get; set; }
    /// <summary>
    /// Default amount of loot cards a player can play per turn
    /// </summary>
    public required int LootPlay { get; set; }
    /// <summary>
    /// Shows whether character cards start the match already tapped
    /// </summary>
    public required bool CharactersStartTapped { get; set; }
    /// <summary>
    /// Initial coin cost to purchase an item from the shop/top of treasure deck
    /// </summary>
    public required int PurchaseCost { get; set; }
}
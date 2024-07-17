namespace FSCore.Matches.Data;

/// <summary>
/// Match data
/// </summary>
public readonly struct MatchData {
    /// <summary>
    /// Players' data
    /// </summary>
    public List<PlayerData> Players { get; }
    /// <summary>
    /// Index of the current player
    /// </summary>
    public int CurPlayerIdx { get; }
    /// <summary>
    /// Current phase name
    /// </summary>
    public string CurrentPhase { get; }
    /// <summary>
    /// Coins left in the coin pool
    /// </summary>
    public int CoinPool { get; }
    /// <summary>
    /// Stack data
    /// </summary>
    public StackData Stack { get; }

    /// <summary>
    /// Loot deck
    /// </summary>
    public DeckData LootDeck { get; }

    public MatchData(Match match) {
        CurPlayerIdx = match.CurPlayerIdx;
        CoinPool = match.CoinPool;
        CurrentPhase = match.CurrentPhase.GetName();
        
        Players = match.Players.Select(p => new PlayerData(p)).ToList();
        Stack = new(match.Stack);
        LootDeck = new(match, match.LootDeck);
    }
}
namespace FSCore.Matches.Data;

public readonly struct PlayerData {
    /// <summary>
    /// Player name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Player index
    /// </summary>
    public int Idx { get; }
    /// <summary>
    /// Amount of coins the player has
    /// </summary>
    public int Coins { get; }
    /// <summary>
    /// Amount of loot cards a player can play
    /// </summary>
    public int LootPlays { get; }
    /// <summary>
    /// Hand size
    /// </summary>
    public int HandSize { get; }
    /// <summary>
    /// Cards, that are visible
    /// </summary>
    public List<CardData> VisibleHandCards { get; }


    public PlayerData(Player player) {
        Name = player.Name;
        Idx = player.Idx;
        Coins = player.Coins;
        LootPlays = player.LootPlays;

        HandSize = player.Hand.Count;
        VisibleHandCards = new();
    }
}
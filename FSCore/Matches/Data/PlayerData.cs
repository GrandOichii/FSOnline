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
    /// <summary>
    /// Character card
    /// </summary>
    public OwnedCardData Character { get; }
    /// <summary>
    /// Owned items
    /// </summary>
    public List<OwnedCardData> Items { get; }

    public PlayerData(Player player) {
        Name = player.Name;
        Idx = player.Idx;
        Coins = player.Coins;
        LootPlays = player.LootPlays;

        Character = new(player.Character);
        HandSize = player.Hand.Count;
        VisibleHandCards = new();
        Items = player.Items.Select(item => new OwnedCardData(item)).ToList();
    }
}
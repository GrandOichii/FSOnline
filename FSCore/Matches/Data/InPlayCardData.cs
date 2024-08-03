namespace FSCore.Matches.Data;

public class InPlayCardData : CardData {
    /// <summary>
    /// Indicates whether the card is tapped
    /// </summary>
    public bool Tapped { get; }
    /// <summary>
    /// In-play card ID
    /// </summary>
    public string IPID { get; }
    /// <summary>
    /// Index of counters (name -> amount)
    /// </summary>
    public Dictionary<string, int> Counters { get; }
    public StatsData? Stats { get; }

    public InPlayCardData(InPlayMatchCard card)
        : base(card.Card)
    {
        IPID = card.IPID;
        Tapped = card.Tapped;

        Counters = [];
        foreach (var c in card.Counters.Values)
            Counters[c.Name] = c.Amount;

        Stats = null;
        if (card.Stats is not null)
            Stats = new(card.Stats);
    }
}
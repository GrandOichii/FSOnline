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

    public InPlayCardData(InPlayMatchCard card)
        : base(card.Card)
    {
        IPID = card.IPID;
        Tapped = card.Tapped;

        Counters = new();
        foreach (var c in card.Counters.Values)
            Counters[c.Name] = c.Amount;
    }
}
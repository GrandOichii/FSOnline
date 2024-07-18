namespace FSCore.Matches.Data;

public class InPlayCardData : CardData {
    /// <summary>
    /// Indicates whether the card is tapped
    /// </summary>
    public bool Tapped { get; }
    /// <summary>
    /// In-play card ID
    /// </summary>
    public string IPID { get; set; }

    public InPlayCardData(InPlayMatchCard card)
        : base(card.Card)
    {
        IPID = card.IPID;
        Tapped = card.Tapped;
    }
}
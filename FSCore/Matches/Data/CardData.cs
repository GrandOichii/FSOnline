namespace FSCore.Matches.Data;

/// <summary>
/// Card data
/// </summary>
public class CardData {
    /// <summary>
    /// Card name(s)
    /// </summary>
    public List<string> Names { get; }
    /// <summary>
    /// Card ID
    /// </summary>
    public string ID { get; }

    public CardData(MatchCard card) {
        ID = card.ID;

        Names = new(card.Names());
    }
}
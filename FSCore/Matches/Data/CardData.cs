namespace FSCore.Matches.Data;

/// <summary>
/// Card data
/// </summary>
public class CardData {
    /// <summary>
    /// Card name(s)
    /// </summary>
    public List<string> Names { get; }

    public CardData(MatchCard card) {

        Names = new(card.Names());
    }
}
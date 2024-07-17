namespace FSCore.Matches.Data;

public class DeckData {
    public int DeckSize { get; }
    // TODO discard

    public DeckData(Match match, Deck deck) {
        DeckSize = deck.Size;
    }
}
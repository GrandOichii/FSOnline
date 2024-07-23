namespace FSCore.Matches.Decks;

/// <summary>
/// Deck of MatchCards
/// </summary>
public class Deck {
    /// <summary>
    /// Parent match
    /// </summary>
    public Match Match { get; }
    /// <summary>
    /// Deck cards
    /// </summary>
    public LinkedList<MatchCard> Cards { get; private set; }
    /// <summary>
    /// Discarded cards
    /// </summary>
    public List<MatchCard>? Discard { get; } = null;

    /// <summary>
    /// Top card
    /// </summary>
    public MatchCard Top => Cards.Last!.Value;
    /// <summary>
    /// Bottom card
    /// </summary>
    public MatchCard Bottom => Cards.First!.Value;
    /// <summary>
    /// Deck size
    /// </summary>
    public int Size => Cards.Count;

    public Deck(Match match, bool hasDiscard) {
        Match = match;

        if (hasDiscard)
            Discard = new();
        Cards = new();
    }

    /// <summary>
    /// Populates the deck with cards
    /// </summary>
    /// <param name="cards">Cards</param>
    public void Populate(List<MatchCard> cards) {
        Cards = new(cards);
    }

    /// <summary>
    /// Shuffles the deck
    /// </summary>
    public void Shuffle() {
        Cards = Common.Shuffled(Cards, Match.Rng);
    }

    /// <summary>
    /// Removes the top <c>amount</c> cards from deck, if size is less than <c>amount</c>, remove all instead
    /// </summary>
    /// <param name="amount">Number of cards to be removed</param>
    /// <returns>Removed cards</returns>
    public List<MatchCard> RemoveTop(int amount) {
        var result = new List<MatchCard>();
        // TODO reshuffle discard into the deck if found to be emoty

        while (amount > 0) {
            if (Size == 0) break;

            result.Add(Top);
            Cards.RemoveLast();
            --amount;
        }

        return result;
    }

    /// <summary>
    /// Places the card into discard (if discard is not null)
    /// </summary>
    /// <param name="card">Match card</param>
    public void PlaceIntoDiscard(MatchCard card) {
        Discard?.Add(card);
    }

    public bool Remove(string id) {
        var card = Cards.FirstOrDefault(c => c.ID == id);
        if (card is not null) {
            Cards.Remove(card);
            return true;
        }
        if (Discard is not null) {
            card = Discard.FirstOrDefault(c => c.ID == id);
            if (card is not null) {
                Discard.Remove(card);
                return true;
            }
        }

        return false;
    }
}
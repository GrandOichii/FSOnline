namespace FSCore.Matches.Decks;

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
    /// Populates the deck using card templates
    /// </summary>
    /// <param name="cards">Card templates</param>
    public void Populate(List<CardTemplate> cards) {
        Cards = new(cards.Select(c => new MatchCard(Match, c)).ToList());
    }

    /// <summary>
    /// Shuffles the deck
    /// </summary>
    public void Shuffle() {
        Cards = Common.Shuffled(Cards, Match.Rng);
    }
}
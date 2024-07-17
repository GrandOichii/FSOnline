namespace FSCore.Matches.Cards;

/// <summary>
/// Hand wrapper for a Match Card
/// </summary>
public class HandMatchCard {
    /// <summary>
    /// Original card
    /// </summary>
    public MatchCard Card { get; }
    /// <summary>
    /// Owner
    /// </summary>
    public Player Owner { get; }
    /// <summary>
    /// State
    /// </summary>
    public HandMatchCardState State { get; private set; }

    public HandMatchCard(MatchCard card, Player owner) {
        Card = card;
        Owner = owner;

        // Initial state
        State = new(this);
    }
}
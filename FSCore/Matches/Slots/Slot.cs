namespace FSCore.Matches.Slots;

/// <summary>
/// A slot for a card
/// </summary>
public abstract class Slot {
    /// <summary>
    /// Visible card
    /// </summary>
    public InPlayMatchCard? Card { get; private set; }
    /// <summary>
    /// Deck source
    /// </summary>
    public Deck Source { get; }
    /// <summary>
    /// Name of the slot
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Slot index
    /// </summary>
    public int Idx { get; }

    public Slot(string name, Deck source, int idx) {
        Source = source;
        Name = name;
        Idx = idx;

        Card = null;
    }

    /// <summary>
    /// Fill the slot with the top card of the source deck
    /// </summary>
    public virtual async Task Fill() {
        if (Card is not null) {
            var match = Card.Card.Match;
            await match.PlaceIntoDiscard(Card.Card);
        }

        var top = Source.RemoveTop(1);
        if (top.Count == 0) {
            Card = null;
            Source.Match.LogDebug("Tried to refill {Slot} slot [{SlotIdx}], but the source deck is empty", Name, Idx);
            return;
        }

        var card = top[0];
        Card = new(card);
        Source.Match.LogDebug("Card {LogName} is put into {Slot} slot [{SlotIdx}]", Card.LogName, Name, Idx);

        await Source.Match.OnCardEnteredPlay(Card);
    }

    public abstract SlotData GetData();

    public virtual async Task ProcessTrigger(QueuedTrigger trigger) {
        if (Card is null) return;

        await Card.ProcessTrigger(trigger);
    }
}
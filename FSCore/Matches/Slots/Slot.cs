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
        var top = Source.RemoveTop(1);
        if (top.Count == 0) {
            Source.Match.LogInfo($"Tried to refill {Name} slot [{Idx}], but the source deck is empty");
            return;
        }

        var card = top[0];
        Card = new(card);
        Source.Match.LogInfo($"Card {Card.LogName} is put into {Name} slot [{Idx}]");

        await Source.Match.OnCardEnteredPlay(Card);
    }

    public abstract SlotData GetData();

    public virtual async Task ProcessTrigger(QueuedTrigger trigger) {
        if (Card is null) return;

        await Card.ProcessTrigger(trigger);
    }
}
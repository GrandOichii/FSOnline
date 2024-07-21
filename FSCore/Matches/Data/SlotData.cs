namespace FSCore.Matches.Data;

/// <summary>
/// Slot data
/// </summary>
public class SlotData {
    /// <summary>
    /// Slot name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Slot index
    /// </summary>
    public int Idx { get; }
    /// <summary>
    /// Card data
    /// </summary>
    public InPlayCardData? Card { get; }

    public SlotData(Slot slot) {
        Name = slot.Name;
        Idx = slot.Idx;
        Card = slot.Card is null
            ? null
            : new(slot.Card);
    }
}
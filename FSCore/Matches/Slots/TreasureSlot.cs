
namespace FSCore.Matches.Slots;

public class TreasureSlot : Slot
{
    public TreasureSlot(Deck source, int idx)
        : base("Treasure", source, idx)
    {

    }

    public override SlotData GetData() => new SlotData(this);
}
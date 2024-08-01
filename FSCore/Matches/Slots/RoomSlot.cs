
namespace FSCore.Matches.Slots;

public class RoomSlot : Slot
{
    public RoomSlot(Deck source, int idx)
        : base("Room", source, idx)
    {

    }

    public override SlotData GetData() => new SlotData(this);
}
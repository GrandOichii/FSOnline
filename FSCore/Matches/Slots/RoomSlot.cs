
namespace FSCore.Matches.Slots;

public class RoomSlot : Slot
{
    public RoomSlot(Deck source, int idx)
        : base("Room", source, idx)
    {

    }

    public override SlotData GetData() => new SlotData(this);

    public async Task ProcessTrigger(QueuedTrigger trigger) {
        if (Card is null) return;

        // TODO
        // if (trigger.Trigger == "item_enter" && LuaUtility.TableGet<InPlayMatchCard>(trigger.Args, "Card").IPID == Card.IPID) {
        //     await Card.ProcessTrigger(trigger);
        //     return;
        // }
    }
}
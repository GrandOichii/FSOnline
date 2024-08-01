
namespace FSCore.Matches.Slots;

public class TreasureSlot : Slot
{
    public TreasureSlot(Deck source, int idx)
        : base("Treasure", source, idx)
    {

    }

    public override SlotData GetData() => new SlotData(this);

    public override async Task ProcessTrigger(QueuedTrigger trigger) {
        if (Card is null) return;
        if (trigger.Trigger != "item_enter" || LuaUtility.TableGet<InPlayMatchCard>(trigger.Args, "Card").IPID != Card.IPID) {
            return;
        }
        await base.ProcessTrigger(trigger);
        // TODO leave play
    }
}
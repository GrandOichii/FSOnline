
namespace FSCore.Matches.Slots;

public class TreasureSlot : Slot
{
    public TreasureSlot(Deck source, int idx)
        : base("Treasure", source, idx)
    {

    }

    public override SlotData GetData() => new SlotData(this);

    public async Task ProcessTrigger(QueuedTrigger trigger) {
        if (Card is null) return;

        if (trigger.Trigger == "item_enter" && LuaUtility.TableGet<InPlayMatchCard>(trigger.Args, "Card").IPID == Card.IPID) {
            await Card.ProcessTrigger(trigger);
            return;
        }

        // TODO leave play
    }
}
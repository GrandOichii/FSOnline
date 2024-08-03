namespace FSCore.Matches.Slots;

public class MonsterSlot : Slot
{
    // TODO add covered monsters

    public MonsterSlot(Deck source, int idx)
        : base("Monster", source, idx)
    {

    }

    // TODO
    public override SlotData GetData() => new(this);

    public override async Task Fill()
    {
        // TODO
        await base.Fill();
    }

    // public MonsterMatchCard GetCard() => (MonsterMatchCard)Card!;
}
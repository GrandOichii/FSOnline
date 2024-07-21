namespace FSCore.Matches;

public enum ModificationLayer {
    COIN_GAIN_AMOUNT = 1,
    ROLL_REPLACEMENT_EFFECTS = 2,
    LOOT_AMOUNT = 3,
    HAND_CARD_VISIBILITY = 4,
    LOOT_PLAY_RESTRICTIONS = 5,
    ITEM_ACTIVATION_RESTRICTIONS = 6,
}

/// <summary>
/// State modifier
/// </summary>
public interface IStateModifier {
    public void UpdateState();
    public void Modify(ModificationLayer layer);
}
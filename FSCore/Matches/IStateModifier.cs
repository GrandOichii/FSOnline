namespace FSCore.Matches;

public enum ModificationLayer {
    COIN_GAIN_AMOUNT = 1,
    ROLL_REPLACEMENT_EFFECTS = 2,
    LOOT_AMOUNT = 3,
    HAND_CARD_VISIBILITY = 4,
    LOOT_PLAY_RESTRICTIONS = 5,
    ITEM_ACTIVATION_RESTRICTIONS = 6,
    PURCHASE_COST = 7,
    LAST = 8,
    ITEM_DESTRUCTION_REPLACEMENT_EFFECTS = 9,
    MOD_MAX_LOOT_PLAYS = 10,
}

/// <summary>
/// State modifier
/// </summary>
public interface IStateModifier {
    public void UpdateState();
    public void Modify(ModificationLayer layer);
}
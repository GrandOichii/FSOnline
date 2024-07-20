namespace FSCore.Matches;

public enum ModificationLayer {
    COIN_GAIN_AMOUNT = 1,
    ROLL_REPLACEMENT_EFFECTS = 2,
    LOOT_AMOUNT = 3,
}

/// <summary>
/// State modifier
/// </summary>
public interface IStateModifier {
    public void UpdateState();
    public void Modify(ModificationLayer layer);
}
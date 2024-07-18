namespace FSCore.Matches;

public enum ModificationLayer {
    COIN_GAIN_AMOUNT = 1,
}

/// <summary>
/// State modifier
/// </summary>
public interface IStateModifier {
    public void UpdateState();
    public void Modify(ModificationLayer layer);
}
namespace FSCore.Matches.Cards;

/// <summary>
/// State of in-play card
/// </summary>
public class InPlayMatchCardState {
    /// <summary>
    /// Original
    /// </summary>
    public InPlayMatchCard Original { get; }
    public List<ActivatedAbilityWrapper> ActivatedAbilities { get; }
    public List<TriggeredAbilityWrapper> TriggeredAbilities { get; }

    #region Replacement effects

    public List<LuaFunction> DestructionReplacementEffects { get;}

    #endregion

    public InPlayMatchCardState(InPlayMatchCard original) {
        Original = original;

        ActivatedAbilities = original.Card.ActivatedAbilities.Select(
            aa => new ActivatedAbilityWrapper(aa)
        ).ToList();

        TriggeredAbilities = original.Card.TriggeredAbilities.Select(
            ta => new TriggeredAbilityWrapper(ta)
        ).ToList();

        DestructionReplacementEffects = [];
    }
}
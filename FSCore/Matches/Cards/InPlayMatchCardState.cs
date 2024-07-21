namespace FSCore.Matches.Cards;

public class InPlayMatchCardState {
    public InPlayMatchCard Original { get; }
    public List<ActivatedAbilityWrapper> ActivatedAbilities { get; }

    public InPlayMatchCardState(InPlayMatchCard original) {
        Original = original;

        ActivatedAbilities = original.Card.ActivatedAbilities.Select(
            aa => new ActivatedAbilityWrapper(aa)
        ).ToList();
    }
}
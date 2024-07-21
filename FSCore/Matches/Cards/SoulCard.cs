namespace FSCore.Matches.Cards;

/// <summary>
/// Soul card
/// </summary>
public class SoulCard {
    public MatchCard Original { get; }

    public SoulCard(MatchCard original) {
        Original = original;
    }

    public int GetSoulValue() {
        return Original.Template.SoulValue;
    }
}
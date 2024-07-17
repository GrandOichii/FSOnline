namespace FSCore.Matches.Cards;

/// <summary>
/// State of a match card
/// </summary>
public class MatchCardState {
    /// <summary>
    /// Parent match card
    /// </summary>
    public MatchCard Parent { get; }
    /// <summary>
    /// Card name(s)
    /// </summary>
    public List<string> Names { get; }

    public MatchCardState(MatchCard parent) {
        Parent = parent;

        Names = new() { parent.Template.Name };
    }
}
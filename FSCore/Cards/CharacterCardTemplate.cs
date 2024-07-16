namespace FSCore.Cards;

/// <summary>
/// Character card template, describes a character's stats as well as their starting item(s)
/// </summary>
public class CharacterCardTemplate : CardTemplate {
    /// <summary>
    /// Starting items of character
    /// </summary>
    public required List<string> StartingItems { get; set; }
}
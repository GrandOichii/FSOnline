namespace FSCore.Cards;

// TODO? could remove StartingItems and make the card script generate them

/// <summary>
/// Character card template, describes a character's stats as well as their starting item(s)
/// </summary>
public class CharacterCardTemplate : CardTemplate {
    /// <summary>
    /// Starting items of character
    /// </summary>
    public required List<string> StartingItems { get; set; }
}
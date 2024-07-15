namespace FSCore.Cards;

/// <summary>
/// Character card template, describes a character's stats as well as their starting item(s)
/// </summary>
public class CharacterCardTemplate : CardTemplate {
    public required List<string> StartingItems { get; set; }
}
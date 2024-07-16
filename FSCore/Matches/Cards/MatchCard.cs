namespace FSCore.Matches.Cards;

public class MatchCard {
    /// <summary>
    /// Parent match
    /// </summary>
    public Match Match { get; }
    /// <summary>
    /// Template, used to create the card
    /// </summary>
    public CardTemplate Template { get; }

    /// <summary>
    /// Name of the card that will be used when logging using system logger
    /// </summary>
    public string LogName => Template.Name; // TODO add IDs

    public MatchCard(Match match, CardTemplate template) {
        Match = match;
        Template = template;

        // TODO create script
    }

    public bool IsCardType(string type) {
        return Template.Type == type;
    }

    public bool IsLoot() => IsCardType("Loot");
}
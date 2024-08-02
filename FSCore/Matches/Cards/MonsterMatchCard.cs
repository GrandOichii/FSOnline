namespace FSCore.Matches.Cards;

/// <summary>
/// Represents a monster card in a monster slot
/// </summary>
public class MonsterMatchCard : InPlayMatchCard
{
    // TODO
    public MonsterMatchCard(MatchCard card)
        : base(card)
    {
        // TODO
    }

    // public MonsterMatchCard(Match match, MonsterCardTemplate template)
    //     : base(new MatchCard(match, template))
    // {

    // }

    public MonsterCardTemplate GetTemplate() {
        return (MonsterCardTemplate)Card.Template;
    }
}
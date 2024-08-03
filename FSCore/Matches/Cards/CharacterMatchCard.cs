
namespace FSCore.Matches.Cards;

public class CharacterMatchCard : OwnedInPlayMatchCard
{
    public CharacterMatchCard(Match match, Player owner, CharacterCardTemplate template)
        : base(new MatchCard(match, template), owner)
    {
        Stats = null;
    }

    public CharacterCardTemplate GetTemplate() {
        return (CharacterCardTemplate)Card.Template;
    }
}
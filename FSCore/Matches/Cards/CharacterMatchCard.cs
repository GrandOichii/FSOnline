
namespace FSCore.Matches.Cards;

public class CharacterMatchCard : OwnedInPlayMatchCard
{
    public CharacterMatchCard(Match match, Player owner, CharacterCardTemplate template)
        : base(new MatchCard(match, template), owner)
    {

    }

    public CharacterCardTemplate GetTemplate() {
        // TODO? is casting bad? this is guaranteed to never throw an error
        return (CharacterCardTemplate)Card.Template;
    }
}

namespace FSCore.Matches.Cards;

public class CharacterMatchCard : InPlayMatchCard
{
    public CharacterMatchCard(Match match, CharacterCardTemplate template)
        : base(new(match, template))
    {

    }

    public CharacterCardTemplate GetTemplate() {
        // TODO? is casting bad? this is guaranteed to never throw an error
        return (CharacterCardTemplate)Card.Template;
    }
}
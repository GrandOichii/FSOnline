
namespace FSCore.Matches.Cards;

public class CharacterMatchCard : OwnedInPlayMatchCard
{
    public CharacterMatchCard(Match match, Player owner, CardTemplate template)
        : base(new MatchCard(match, template), owner)
    {
        Stats = null;
    }
}
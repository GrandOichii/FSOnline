namespace FSCore.Tests.Setup.Players;

public interface IProgrammedPlayerSetup
{
    public Task Do(Match match, int playerIdx);
}

public class GainItemPPSetup(string itemKey, DeckType? deck = null) : IProgrammedPlayerSetup
{
    public async Task Do(Match match, int playerIdx)
    {
        var template = await match.CardMaster.Get(itemKey);
        var card = new MatchCard(match, template, deck);
        var result = new OwnedInPlayMatchCard(card, match.GetPlayer(playerIdx));
        await match.PlaceOwnedCard(result);
    }
}
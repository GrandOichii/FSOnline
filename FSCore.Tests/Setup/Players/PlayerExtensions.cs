namespace FSCore.Tests.Setup.Players;

public static class PlayerExtensions
{
    public static List<OwnedInPlayMatchCard> StartingItems(this Player player) => player.Items
        .Where(i => player.Character.Card.StartingItemKeys.Contains(i.Card.Template.Key))
        .ToList();
}

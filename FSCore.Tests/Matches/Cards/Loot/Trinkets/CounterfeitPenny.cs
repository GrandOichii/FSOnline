namespace FSCore.Tests.Matches.Cards.Loot.Trinkets;

public class CounterfeitPennyTests
{
    [Theory]
    [InlineData("a-penny-b", 1)]
    [InlineData("2-cents-b", 2)]
    [InlineData("3-cents-b", 3)]
    [InlineData("4-cents-b", 4)]
    [InlineData("a-nickel-b", 5)]
    [InlineData("a-dime-b", 10)]
    public async Task WithBasicCoinCards(string coinCardKey, int coinValue)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "counterfeit-penny-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(3)
            .LootPlay(3)
            .ConfigLootDeck(d => d.Add(coinCardKey, 2).Add(cardKey))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(cardKey)
                .AutoPassUntilEmptyStack()
                .PlayLootCard(coinCardKey)
                .AutoPassUntilEmptyStack()
                .RemoveItem(cardKey)
                .PlayLootCard(coinCardKey)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner()
            .HasCoins(2 * coinValue + 1);
        match.AssertCoinsInBank(config.CoinPool - 2 * coinValue - 1);
    }
}
namespace FSCore.Tests;

public class CoinCardTests
{
    [Theory]
    [InlineData("a-penny-b", 1)]
    [InlineData("2-cents-b", 2)]
    [InlineData("3-cents-b", 3)]
    [InlineData("4-cents-b", 4)]
    [InlineData("a-nickel-b", 5)]
    [InlineData("a-dime-b", 10)]
    public async Task BasicCoinCards(string cardKey, int coinValue)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(cardKey, 10).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(cardKey)
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
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertHasCoins(mainPlayerIdx, coinValue);
        match.AssertCoinsInBank(config.CoinPool - coinValue);
    }


}
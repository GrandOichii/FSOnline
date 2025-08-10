namespace FSCore.Tests.Matches.Loot;

public class BlankRuneTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(4, 4)]
    [InlineData(6, 6)]
    public async Task CoinGain(int rollResult, int coinValue)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "blank-rune-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(cardKey, 10).Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(rollResult)
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

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertHasCoins(mainPlayerIdx, coinValue);
        match.AssertHasCoins(1 - mainPlayerIdx, coinValue);
        match.AssertCoinsInBank(config.CoinPool - 2 * coinValue);
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(5, 5)]
    public async Task Loot(int rollResult, int lootDraw)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "blank-rune-b";
        var lootDeckSize = 100;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(cardKey, lootDeckSize).Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(rollResult)
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

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertCardsInHand(mainPlayerIdx, lootDraw);
        match.AssertCardsInHand(1 - mainPlayerIdx, lootDraw);
        match.AssertCardsInLootDeck(lootDeckSize - 2 * lootDraw - 1);
        match.AssertCardsInLootDiscard(1);
    }

    // TODO add 3 tests
}
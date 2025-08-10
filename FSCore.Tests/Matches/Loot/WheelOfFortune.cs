namespace FSCore.Tests.Matches.Loot;

public class WheelOfFortuneTests
{
    private static readonly string CARD_KEY = "x-wheel-of-fortune-b";

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 5)]
    public async Task CoinRolls(int roll, int coinValue)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(roll)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
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
        match.AssertCoinsInBank(config.CoinPool - coinValue);
    }

    // TODO add test for 2

    [Fact]
    public async Task Roll3()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(3)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
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
        match.AssertCardsInHand(mainPlayerIdx, 3);
        match.AssertCardsInLootDeck(lootDeckSize - 3 - 1);
    }

    [Fact]
    public async Task Roll4()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var initialCoins = 4;
        var config = new MatchConfigBuilder()
            .InitialCoins(initialCoins)
            .InitialLoot(0)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(4)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
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
        match.AssertHasCoins(mainPlayerIdx, 0);
        match.AssertCoinsInBank(config.CoinPool - initialCoins);
    }

    // TODO add roll 6
}
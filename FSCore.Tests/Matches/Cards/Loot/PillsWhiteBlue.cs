namespace FSCore.Tests.Matches.Loot;

public class PillsWhiteBlueTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 3)]
    [InlineData(4, 3)]
    public async Task LootRolls(int rollResult, int lootAmount)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "pills-white-blue-b";
        var lootDeckSize = 10;
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
        match.AssertCardsInHand(mainPlayerIdx, lootAmount);
        match.AssertCardsInLootDeck(lootDeckSize - 1 - lootAmount);
    }

    [Theory]
    [InlineData(5, 2)]
    [InlineData(6, 2)]
    public async Task DiscardRolls(int rollResult, int expectedHandSize)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "pills-white-blue-b";
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(3)
            .ConfigLootDeck().Add(cardKey, lootDeckSize).Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(rollResult)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(cardKey)
                .Choose.CardInHand(0)
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
        match.AssertCardsInHand(mainPlayerIdx, expectedHandSize);
    }
}
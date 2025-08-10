namespace FSCore.Tests.Matches.Treasures;

public class BookOfSinTests
{
    private static readonly string CARD_KEY = "book-of-sin-b";
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public async Task CoinRolls(int roll, int coins)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .ConfigTreasureDeck().Add(CARD_KEY).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .GainTreasure(1)
                .Pass()
                .AutoPassUntilMyTurn()
                .ActivateOwnedItem(CARD_KEY, 0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(roll)
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
        match.AssertHasCoins(mainPlayerIdx, coins);
        match.AssertCoinsInBank(config.CoinPool - coins);
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(4, 1)]
    public async Task LootRolls(int roll, int loot)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootCard = "a-penny-b";
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .ConfigLootDeck().Add(lootCard, lootDeckSize).Done()
            .ConfigTreasureDeck().Add(CARD_KEY).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .GainTreasure(1)
                .Pass()
                .AutoPassUntilMyTurn()
                .ActivateOwnedItem(CARD_KEY, 0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(roll)
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
        match.AssertCardsInHand(mainPlayerIdx, loot);
        match.AssertCardsInLootDeck(lootDeckSize - loot);
    }

    [Theory]
    [InlineData(5, 3)]
    [InlineData(6, 3)]
    public async Task HealthGainRolls(int roll, int health)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .ConfigTreasureDeck().Add(CARD_KEY).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .GainTreasure(1)
                .Pass()
                .AutoPassUntilMyTurn()
                .ActivateOwnedItem(CARD_KEY, 0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(roll)
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
        match.AssertHasHealth(mainPlayerIdx, health);
        match.AssertHasItemCount(mainPlayerIdx, 2);
    }
}
namespace FSCore.Tests.Matches.Cards.Loot;

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
        var opponentIdx = 1 - mainPlayerIdx;
        var cardKey = "blank-rune-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck(d => d.Add(cardKey, 10))
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
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner()
            .HasCoins(coinValue);
        match.AssertPlayer(opponentIdx)
            .HasCoins(coinValue);
        match.AssertCoinsInBank(config.CoinPool - 2 * coinValue);
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(5, 5)]
    public async Task Loot(int rollResult, int lootDraw)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var cardKey = "blank-rune-b";
        var lootDeckSize = 100;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck(d => d.Add(cardKey, lootDeckSize))
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
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner()
            .HasCardsInHand(lootDraw);
        match.AssertPlayer(opponentIdx)
            .HasCardsInHand(lootDraw);
        match.AssertCardsInLootDeck(lootDeckSize - 2 * lootDraw - 1);
        match.AssertCardsInLootDiscard(1);
    }

    // TODO add 3 tests
}
namespace FSCore.Tests.Matches.Treasures;

public class BreakfastDinnerTests
{    
    [Theory]
    [InlineData("breakfast-b")]
    [InlineData("dinner-b")]
    public async Task Play(string cardKey)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .ConfigTreasureDeck().Add(cardKey).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .GainTreasure(1)
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
        match.AssertHasItemCount(mainPlayerIdx, 2);
        match.AssertPlayerHasHealth(mainPlayerIdx, 3);
    }
}
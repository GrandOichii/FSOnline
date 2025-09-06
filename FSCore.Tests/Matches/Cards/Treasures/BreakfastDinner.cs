namespace FSCore.Tests.Matches.Cards.Treasures;

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
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(cardKey)
            .ConfigActions()
                .AssertIsCurrentPlayer()
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
            .HasItemCount(2)
            .HasHealth(3);
    }
}
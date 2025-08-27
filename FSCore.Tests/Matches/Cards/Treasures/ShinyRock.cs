namespace FSCore.Tests.Matches.Treasures;

public class ShinyRockTests
{
    private static readonly string CARD_KEY = "shiny-rock-b";
    
    [Fact]
    public async Task Trigger()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var activatable = "the-butter-bean-b2";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .HasItemAtStart(activatable)
            .ConfigActions()
                .ActivateOwnedItem(activatable, 0)
                .Choose.Me()
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(1)
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
        match.AssertHasCoins(mainPlayerIdx, 1);
    }
}
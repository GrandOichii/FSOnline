namespace FSCore.Tests.Matches.Characters;

public class IsaacTests
{
    
    [Fact]
    public async Task Activate()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var activatable = "the-butter-bean-b2";
        var startingItem = "the-d6-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(activatable)
            .ConfigActions()
                .ActivateOwnedItem(activatable, 0)
                .ActivateOwnedItem(startingItem)
                    .Choose.DiceRoll(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(1)
            .Then(6)
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == activatable);

        // Assert
        match.AssertPlayer(mainPlayerIdx).IsWinner();
        item.ShouldNotBeNull();
        item.Tapped.ShouldBeFalse();
    }
}
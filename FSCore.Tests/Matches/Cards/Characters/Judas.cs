namespace FSCore.Tests.Matches.Characters;

public class JudasTests
{
    [Fact]
    public async Task Activate_PlusOne()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var activatable = "the-butter-bean-b2";
        var startingItem = "book-of-belial-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("judas-b")
            .HasItemAtStart(activatable)
            .ConfigActions()
                .ActivateOwnedItem(activatable, 0)
                .ActivateOwnedItem(startingItem)
                    .Choose.DiceRoll(0)
                    .Choose.Option(0) // choose +1
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(5)
            .Then(6)
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("isaac-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx);
        await match.AddPlayers(players);

        // Act
        await match.Run();
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == activatable);

        // Assert
        match.AssertPlayer(mainPlayerIdx).IsWinner();
        item.ShouldNotBeNull();
        item.Tapped.ShouldBeFalse();
    }

    [Fact]
    public async Task Activate_MinusOne()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var activatable = "the-butter-bean-b2";
        var startingItem = "book-of-belial-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("judas-b")
            .HasItemAtStart(activatable)
            .ConfigActions()
                .ActivateOwnedItem(activatable, 0)
                .ActivateOwnedItem(startingItem)
                    .Choose.DiceRoll(0)
                    .Choose.Option(1)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(6)
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("isaac-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == activatable);

        // Assert
        match.AssertPlayer(mainPlayerIdx).IsWinner();
        item.ShouldNotBeNull();
        item.Tapped.ShouldBeTrue();
    }
}
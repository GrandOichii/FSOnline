namespace FSCore.Tests.Matches.Treasures;

public class TheButterBeanTests
{    
    private static readonly string CARD_KEY = "the-butter-bean-b2";

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task DoesntRecharge(int roll)
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
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == CARD_KEY);

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        item.ShouldNotBeNull();
        item.Tapped.ShouldBeTrue();
    }

    [Fact]
    public async Task Recharges()
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
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == CARD_KEY);

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        item.ShouldNotBeNull();
        item.Tapped.ShouldBeFalse();
    }
}
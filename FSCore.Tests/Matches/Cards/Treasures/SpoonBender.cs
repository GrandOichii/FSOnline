namespace FSCore.Tests.Matches.Cards.Treasures;

public class SpoonBenderTests
{
    private static readonly string CARD_KEY = "spoon-bender-b";

    [Fact]
    public async Task Activate_SetTo1()
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

        var mainPlayer = new ProgrammedPlayerControllerBuilder("judas-b")
            .HasItemAtStart(activatable)
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .ActivateOwnedItem(activatable, 0)
                .ActivateOwnedItem(CARD_KEY)
                    .Choose.DiceRoll(0)
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

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner();
        match.AssertSingleItem(mainPlayerIdx, activatable)
            .IsUntapped();
    }
}
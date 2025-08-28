namespace FSCore.Tests.Matches.Monsters;

public class SquirtTests
{
    // TODO make more compact
    
    [Fact]
    public async Task CheckRewards()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .InitialMonsterSlots(1)
            .ConfigLootDeck().Add("a-dime-b", 10).Done()
            .ConfigMonsterDeck().AddMonster("squirt-b").Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .DeclareAttack(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(6)
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

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertCardsInHand(mainPlayerIdx, 1);
    }
}
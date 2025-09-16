namespace FSCore.Tests.Matches.Cards.Monsters;

public class HopperTests
{
    [Fact]
    public static async Task CheckNoDamageRoll()
    {
        // Arrange
        var monsterCardKey = "hopper-b";
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster(monsterCardKey))
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
            .Then(5)
            .Then(4)
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
            .HasCoins(3);
    }

}

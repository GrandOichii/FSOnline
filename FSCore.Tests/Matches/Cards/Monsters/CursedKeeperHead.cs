namespace FSCore.Tests.Matches.Monsters;

public class CursedKeeperHeadTests
{
    [Fact]
    public static async Task CheckCombatRoll()
    {
        // Arrange
        var monsterCardKey = "cursed-keeper-head-b";
        var mainPlayerIdx = 0;
        var initialCoins = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(initialCoins)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck().AddMonster(monsterCardKey).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .DeclareAttack(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(1)
            .Default(6)
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
            .HasCoins(initialCoins - 2 + 6);
    }

    // TODO add tests for non-combat rolls
}

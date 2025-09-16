namespace FSCore.Tests.Matches.Custom;

public class RequiredAttackTests
{
    [Fact]
    public static async Task Test1()
    {
        // kill Conquest, kill hopper, check that everything is ok and can pass the turn

        // Arrange
        var monsterCardKey1 = "conquest-b";
        var monsterCardKey2 = "hopper-b";
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster(monsterCardKey1).AddMonster(monsterCardKey2))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .DeclareAttack(0)
                .AutoPassUntilCant()
                .AssertOptions(o => o.CanOnlyDeclareAttack())
                .DeclareAttack(0)
                .AutoPassUntilEmptyStack()
                .AssertOptions(o => o.CanPass())
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .ThenNTimes(3, 4)
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
            .HasNoRequiredAttacks()
            .HasSoulCard(monsterCardKey1);
    }
}
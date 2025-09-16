namespace FSCore.Tests.Matches.Cards.Monsters;

public class EnvyTests
{
    [Fact]
    public static async Task KillCheck()
    {
        // Arrange
        var monsterCardKey = "envy-b";
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
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
                .AutoPassUntilCant()
                .AssertOptions(o => o.CanOnlyDeclareAttack())
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(5)
            .Then(5)
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
            .HasCoins(1)
            .HasRequiredAttacks(1)
            .HasSoulCard(monsterCardKey);
    }

}

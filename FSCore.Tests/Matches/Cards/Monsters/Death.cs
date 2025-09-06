namespace FSCore.Tests.Matches.Cards.Monsters;

public class DeathTests
{
    [Fact]
    public static async Task KillCheck()
    {
        // Arrange
        var monsterCardKey = "death-b";
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .InitialMonsterSlots(1)
            .ConfigTreasureDeck().Add("the-butter-bean-b2").Done() // TODO add configuration
            .ConfigMonsterDeck().AddMonster(monsterCardKey).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .DeclareAttack(0)
                    .Choose.Player(opponentIdx)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
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
            .HasItemCount(2)
            .HasSoulCard(monsterCardKey);
        match.AssertPlayer(opponentIdx)
            .IsDead();
    }

}

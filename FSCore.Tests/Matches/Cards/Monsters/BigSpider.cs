namespace FSCore.Tests.Matches.Cards.Monsters;

public class BigSpiderTests
{
    private static readonly string CARD_KEY = "big-spider-b";

    [Fact]
    public static async Task DeathTrigger()
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
            .ConfigLootDeck(d => d.Add("a-dime-b", 10))
            .ConfigMonsterDeck(d => d.AddMonster(CARD_KEY))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .DeclareAttack(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .ThenNTimes(4, 3)
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
            .HasCardsInHand(1)
            .HasAttackOpportunities(1)
            .CanAttackTopOfMonsterDeck()
            .CannotAttackMonsterSlots();
    }

}

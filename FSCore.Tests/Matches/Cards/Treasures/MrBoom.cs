namespace FSCore.Tests.Matches.Cards.Treasures;

public class MrBoomTests
{
    private static readonly string CARD_KEY = "mr-boom-b";

    [Fact]
    public async Task Base()
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
            .ConfigMonsterDeck().AddMonster("clotty-b").Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .ActivateOwnedItem(CARD_KEY, 0)
                    .Choose.MonsterInSlot(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner();
        match.AssertMonsterInSlot(0)
            .HasDamage(1);
    }
}
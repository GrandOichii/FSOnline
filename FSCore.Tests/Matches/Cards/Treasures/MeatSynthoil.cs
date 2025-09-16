namespace FSCore.Tests.Matches.Cards.Treasures;

public class MeatSynthoilTests
{
    [Theory]
    [InlineData("meat-b")]
    [InlineData("synthoil-b")]
    public async Task Attack(string cardKey)
    {
        // Arrange
        var monsterKey = "little-horn-b";
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster(monsterKey))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(cardKey)
            .ConfigActions()
                .DeclareAttack(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .ThenNTimes(5, 2)
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
            .HasSoulCard(monsterKey);
    }

    [Theory]
    [InlineData("meat-b")]
    [InlineData("synthoil-b")]
    public async Task DoesntRecharge(string cardKey)
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

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(cardKey)
            .HasItemAtStart(activatable)
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .ActivateOwnedItem(activatable, 0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
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
            .IsWinner();
        match.AssertSingleItem(mainPlayerIdx, activatable)
            .IsTapped();
    }


}
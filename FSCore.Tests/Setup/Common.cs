namespace FSCore.Tests.Setup;

public static class Common
{
    // Common roll treasures
    public static async Task SetupTreasureRollTest(
        string cardKey,
        int roll,
        Action<TestMatch, int> asserts
    )
    {
        // Arrange
        var activatable = "the-butter-bean-b2";
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .ConfigLootDeck(d => d.Add("a-penny-b", 10))
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
            .Then(roll)
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
        asserts.Invoke(match, mainPlayerIdx);
    }

    public static async Task SetupRewardsTest(
        string monsterCardKey,
        Action<TestMatch, int> asserts,
        int roll = 6
    )
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
            .ConfigLootDeck(d => d.Add("a-dime-b", 10)) // TODO add configuration
            .ConfigTreasureDeck(d => d.Add("the-butter-bean-b2")) // TODO add configuration
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
            .Default(roll)
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
        asserts.Invoke(match, mainPlayerIdx);
    }
}
namespace FSCore.Tests.Matches.Loot;

public class DiceShardTests
{
    private static readonly string CARD_KEY = "dice-shard-b";

    [Fact]
    public async Task RerollTo6()
    {
        // Arrange
        var treasureKey = "the-butter-bean-b2";
        var firstLootKey = "xvii-the-stars-b";
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialTreasureSlots(0)
            .LootStepLootAmount(2)
            .LootPlay(2)
            .ConfigLootDeck().Add(CARD_KEY, 1).Done()
            .ConfigLootDeck().Add(firstLootKey, 1).Done()
            .ConfigTreasureDeck().Add(treasureKey).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(firstLootKey)
                .AutoPassUntilEmptyStack()
                .ActivateOwnedItem(treasureKey)
                .PlayLootCard(CARD_KEY)
                .Choose.DiceRoll(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(1)
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
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == treasureKey);

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        item.ShouldNotBeNull();
        item.Tapped.ShouldBeFalse();
    }

    [Fact]
    public async Task RerollTo1()
    {
        // Arrange
        var treasureKey = "the-butter-bean-b2";
        var firstLootKey = "xvii-the-stars-b";
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialTreasureSlots(0)
            .LootStepLootAmount(2)
            .LootPlay(2)
            .ConfigLootDeck().Add(CARD_KEY, 1).Done()
            .ConfigLootDeck().Add(firstLootKey, 1).Done()
            .ConfigTreasureDeck().Add(treasureKey).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(firstLootKey)
                .AutoPassUntilEmptyStack()
                .ActivateOwnedItem(treasureKey)
                .PlayLootCard(CARD_KEY)
                .Choose.DiceRoll(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(6)
            .Then(1)
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == treasureKey);

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        item.ShouldNotBeNull();
        item.Tapped.ShouldBeTrue();
    }
}
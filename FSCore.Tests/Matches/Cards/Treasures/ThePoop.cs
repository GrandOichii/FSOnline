namespace FSCore.Tests.Matches.Treasures;

public class ThePoopTests
{
    private static readonly string CARD_KEY = "the-poop-b";
    
    [Fact]
    public async Task DamageTrigger()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var activatable = "razor-blade-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .HasItemAtStart(activatable)
            .ConfigActions()
                .ActivateOwnedItem(activatable)
                    .Choose.Me()
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
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == CARD_KEY);

        // Assert
        match.AssertPlayer(mainPlayerIdx).IsWinner();
        item.ShouldNotBeNull();
        item.GetCountersCount().ShouldBe(1);
    }

    [Fact]
    public async Task NoDamageTrigger()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var activatable = "razor-blade-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .HasItemAtStart(activatable)
            .ConfigActions()
                .PreventDamage(1)
                .ActivateOwnedItem(activatable)
                    .Choose.Me()
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
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == CARD_KEY);

        // Assert
        match.AssertPlayer(mainPlayerIdx).IsWinner();
        item.ShouldNotBeNull();
        item.GetCountersCount().ShouldBe(0);
    }

    [Fact]
    public async Task CantActivate()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .AssertCantActivateItem(CARD_KEY)
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
        match.AssertPlayer(mainPlayerIdx).IsWinner();
    }

    [Fact]
    public async Task Activate()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var activatable = "razor-blade-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .HasItemAtStart(activatable)
            .ConfigActions()
                .PlaceCounters(CARD_KEY, 1)
                .ActivateOwnedItem(activatable)
                    .Choose.Me()
                .ActivateOwnedItem(CARD_KEY)
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
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == CARD_KEY);

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner()
            .HasDamage(0);
    }
}
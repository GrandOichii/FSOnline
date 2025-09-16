namespace FSCore.Tests.Matches.Cards.Treasures;

public class ChampionBeltTests
{
    private static readonly string CARD_KEY = "champion-belt-b";

    [Fact]
    public async Task ShouldHave2AO()
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
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
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
            .HasAttackOpportunities(2);
    }

    [Fact]
    public async Task ShouldGainAOWhenGainingItem()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootCardKey = "xvii-the-stars-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .LootStepLootAmount(1)
            .LootPlay(1)
            .InitialTreasureSlots(0)
            .ConfigLootDeck().Add("xvii-the-stars-b", 10).Done()
            .ConfigTreasureDeck().Add(CARD_KEY).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .Assert(
                    p => p.HasAttackOpportunities(1)
                )
                .PlayLootCard(lootCardKey)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
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
            .HasAttackOpportunities(2);
    }

    [Fact]
    public async Task ShouldLoseAOWhenLosingItem()
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
                .Assert(p => p.HasAttackOpportunities(2))
                .RemoveItem(CARD_KEY)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
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
            .HasAttackOpportunities(1);
    }
}
namespace FSCore.Tests.Matches.Loot;

public class TheStarsTests
{
    private static readonly string CARD_KEY = "xvii-the-stars-b";

    [Fact]
    public async Task Play()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var itemKey = "breakfast-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialTreasureSlots(0)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .ConfigTreasureDeck().Add(itemKey).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
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
            .IsWinner()
            .HasItemCount(2)
            .HasItem(itemKey);
    }
}
namespace FSCore.Tests.Matches.Cards.BonusSouls;

public class SoulOfGreedTests
{
    private static readonly string CARD_KEY = "soul-of-greed-b";
    
    [Fact]
    public async Task Base()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootKey = "a-penny-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(24)
            .InitialLoot(0)
            .InitialTreasureSlots(0)
            .ConfigLootDeck(d => d.Add(lootKey))
            .ConfigBonusSouls(d => d.Add(CARD_KEY))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .PlayLootCard(lootKey)
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
            .HasCoins(25)
            .HasSoulCard(CARD_KEY);
    }
}
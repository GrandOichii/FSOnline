namespace FSCore.Tests.Matches.Cards.BonusSouls;

public class SoulOfGluttonyTests
{
    private static readonly string CARD_KEY = "soul-of-gluttony-b";
    
    [Fact]
    public async Task Base()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootKey = "a-penny-b";
        var config = new MatchConfigBuilder()
            .InitialLoot(9)
            .InitialTreasureSlots(0)
            .ConfigLootDeck().Add(lootKey, 100).Done()
            .ConfigBonusSouls().Add(CARD_KEY).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
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
            .HasCardsInHand(10)
            .HasSoulCard(CARD_KEY);
    }
}
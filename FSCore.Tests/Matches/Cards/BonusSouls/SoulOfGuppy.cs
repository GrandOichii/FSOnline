namespace FSCore.Tests.Matches.Cards.BonusSouls;

public class SoulOfGuppyTests
{
    private static readonly string CARD_KEY = "soul-of-guppy-b";
    private static readonly List<string> GUPPY_CARDS =
    [
        "guppys-paw-b",
        "guppys-head-b",
        "guppys-tail-b2",
    ];

    [Fact]
    public async Task Base()
    {
        var lootKey = "xvii-the-stars-b";
        var itemCount = GUPPY_CARDS.Count;

        for (int i = 0; i < itemCount; ++i)
        {
            for (int ii = i + 1; ii < itemCount; ++ii)
            {
                // Arrange
                var firstKey = GUPPY_CARDS[i];
                var secondKey = GUPPY_CARDS[ii];
                var mainPlayerIdx = 0;
                var config = new MatchConfigBuilder()
                    .InitialLoot(0)
                    .LootStepLootAmount(2)
                    .LootPlay(2)
                    .InitialTreasureSlots(0)
                    .ConfigLootDeck().Add(lootKey, 2).Done()
                    .ConfigBonusSouls().Add(CARD_KEY).Done()
                    .ConfigTreasureDeck().Add(firstKey).Add(secondKey).Done()
                    .Build();

                var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
                    .ConfigActions()
                        .PlayLootCard(lootKey)
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
                    .HasSoulCard(CARD_KEY);
            }
        }
    }

    
}
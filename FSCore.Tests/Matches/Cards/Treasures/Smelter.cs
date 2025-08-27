namespace FSCore.Tests.Matches.Treasures;

public class SmelterTests
{    
    private static readonly string CARD_KEY = "smelter-b";

    [Theory]
    [InlineData(1, 3)]
    [InlineData(2, 6)]
    [InlineData(3, 9)]
    public async Task Activate(int activations, int coins)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(3)
            .LootStepLootAmount(0)
            .LootPlay(0)
            .InitialTreasureSlots(0)
            .ConfigLootDeck().Add("a-penny-b", 3).Done()
            .Build();

        var builder = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .AssertIsCurrentPlayer();
                
        for (int i = 0; i < activations; ++i) {
            builder
                .ActivateOwnedItem(CARD_KEY, 0)
                .Choose.CardInHand(0);
        }

        var mainPlayer = builder
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
        var item = match.Match.GetPlayer(mainPlayerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == CARD_KEY);

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertCardsInHand(mainPlayerIdx, 3 - activations);
        match.AssertHasCoins(mainPlayerIdx, coins);
    }
}
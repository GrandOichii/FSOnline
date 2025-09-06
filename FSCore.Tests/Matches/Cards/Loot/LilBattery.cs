namespace FSCore.Tests.Matches.Cards.Loot;

public class LilBatteryTests
{
    [Fact]
    public async Task Play()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "lil-battery-b";
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(cardKey, lootDeckSize).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("the-keeper-b2")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .ActivateOwnedItem("wooden-nickel-b2")
                    .Choose.Me()
                .AutoPassUntilEmptyStack()
                .PlayLootCard(cardKey)
                    .Choose.StartingItem()
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

        // Asser
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner();
        match.AssertStartingItem(mainPlayerIdx)
            .IsUntapped();
        // TODO add assertions
    }

}
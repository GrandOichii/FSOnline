namespace FSCore.Tests.Matches.Loot;

public class TemperanceTests
{
    private static readonly string CARD_KEY = "xiv-temperance-b";

    [Fact]
    public async Task FirstOption()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(CARD_KEY, 10).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
                    .Choose.Option(0)
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
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertHasCoins(mainPlayerIdx, 4);
        match.AssertPlayerHasHealth(mainPlayerIdx, 1);
    }

    [Fact]
    public async Task SecondOption()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(CARD_KEY, 10).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PreventDamage(1)
                .PlayLootCard(CARD_KEY)
                    .Choose.Option(1)
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
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertHasCoins(mainPlayerIdx, 8);
        match.AssertPlayerHasHealth(mainPlayerIdx, 1);
    }
}
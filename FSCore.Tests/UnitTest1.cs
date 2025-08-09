namespace FSCore.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .StartingPlayer(mainPlayerIdx)
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add("a-nickel-b", 10).Done()
            .Build();
        
        var mainPlayer = new ProgrammedPlayerControllerBuilder()
            .SetCharacter("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
        ];

        var match = new TestMatch(config, players);

        // Act
        await match.Run();

        // Assert
        match.AssertIsWinner(mainPlayerIdx);
        match.AssertHasCoins(mainPlayerIdx, 5);
        match.AssertCoinsInBank(95);
    }
}
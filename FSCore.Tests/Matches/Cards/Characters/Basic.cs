namespace FSCore.Tests.Matches.Characters;

public class BasicCharacterTests
{
    [Theory]
    [InlineData("isaac-b")]
    [InlineData("judas-b")]
    [InlineData("samson-b")]
    [InlineData("lazarus-b")]
    [InlineData("maggy-b")]
    [InlineData("cain-b")]
    public async Task Tap(string characterKey)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder(characterKey)
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .ActivateCharacter()
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
            .HasLootPlays(2);
    }
}
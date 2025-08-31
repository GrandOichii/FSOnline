namespace FSCore.Tests.Matches.Loot;

public class DeathTests
{
    [Fact]
    public async Task OnOpponent()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var cardKey = "xiii-death-b";
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .DisableDeathPenalty()
            .ConfigLootDeck().Add(cardKey, lootDeckSize).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(cardKey)
                    .Choose.Player(opponentIdx)
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
            .IsWinner();
        match.AssertPlayer(opponentIdx)
            .IsDead();
    }

    // ! can't test on self because the turn will end and the player will revive
}
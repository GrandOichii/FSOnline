namespace FSCore.Tests.Matches.Cards.Loot;

public class TheChariotTests
{
    [Fact]
    public async Task OnSelf()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "vii-the-chariot-b";
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck(d => d.Add(cardKey, lootDeckSize))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(cardKey)
                    .Choose.Me()
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
            .HasHealth(3)
            .HasAttack(2);
    }

    [Fact]
    public async Task OnOpponent()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var cardKey = "vii-the-chariot-b";
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck(d => d.Add(cardKey, lootDeckSize))
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
            .HasHealth(3)
            .HasAttack(2);
    }

}
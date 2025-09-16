namespace FSCore.Tests.Matches.Cards.Loot;

public class BombTests
{
    [Fact]
    public async Task OnSelf()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "bomb-b";
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster("clotty-b"))
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
            .HasDamage(1);
    }

    [Fact]
    public async Task OnOpponent()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "bomb-b";
        var opponentIdx = 1 - mainPlayerIdx;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster("clotty-b"))
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
            .HasDamage(1);
    }

    [Fact]
    public async Task OnMonster()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var cardKey = "bomb-b";
        var opponentIdx = 1 - mainPlayerIdx;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster("clotty-b"))
            .ConfigLootDeck(d => d.Add(cardKey, lootDeckSize))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(cardKey)
                    .Choose.MonsterInSlot(0)
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
        match.AssertMonsterInSlot(0)
            .HasDamage(1);
    }
}
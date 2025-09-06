namespace FSCore.Tests.Matches.Cards.Treasures;

public class GoldenRazorTests
{
    private static readonly string CARD_KEY = "golden-razor-blade-b";

    [Fact]
    public async Task CantActivate()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(4)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck().AddMonster("clotty-b").Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .AssertCantActivateItem(CARD_KEY)
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
    }

    [Fact]
    public async Task OnSelf()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var config = new MatchConfigBuilder()
            .InitialCoins(10)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck().AddMonster("clotty-b").Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .ActivateOwnedItem(CARD_KEY)
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
            .HasCoins(5)
            .HasDamage(1);
    }

    [Fact]
    public async Task OnOpponent()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var config = new MatchConfigBuilder()
            .InitialCoins(10)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck().AddMonster("clotty-b").Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .ActivateOwnedItem(CARD_KEY)
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
            .IsWinner()
            .HasCoins(5);
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
            .InitialCoins(10)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck().AddMonster("clotty-b").Done()
            .ConfigLootDeck().Add(cardKey, lootDeckSize).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(CARD_KEY)
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .ActivateOwnedItem(CARD_KEY)
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
            .IsWinner()
            .HasCoins(5);
        match.AssertMonsterInSlot(0)
            .HasDamage(1);
    }
}
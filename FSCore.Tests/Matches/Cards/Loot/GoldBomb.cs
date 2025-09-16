namespace FSCore.Tests.Matches.Cards.Loot;

public class GoldBombTests
{
    private static readonly string CARD_KEY = "gold-bomb-b";

    [Fact]
    public async Task OnSelf()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster("clotty-b"))
            .ConfigLootDeck(d => d.Add(CARD_KEY, lootDeckSize))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PreventDamage(3)
                .PlayLootCard(CARD_KEY)
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
            .HasNoDamagePreventors()
            .HasNoDamage();
    }

    [Fact]
    public async Task OnOpponent()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster("clotty-b"))
            .ConfigLootDeck(d => d.Add(CARD_KEY, lootDeckSize))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
                    .Choose.Player(opponentIdx)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            new ProgrammedPlayerControllerBuilder("judas-b")
                .ConfigActions()
                    .PreventDamage(3)
                    .AutoPass()
                    .Done()
                .Build(),
        ];

        var match = new TestMatch(config, mainPlayerIdx);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner();
        match.AssertPlayer(opponentIdx)
            .HasNoDamagePreventors()
            .HasNoDamage();
    }

    [Fact]
    public async Task OnMonster()
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigMonsterDeck(d => d.AddMonster("monstro-b"))
            .ConfigLootDeck(d => d.Add(CARD_KEY, lootDeckSize))
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
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
            .HasDamage(3);
    }
}
namespace FSCore.Tests.Matches.Loot;

public class TheHighPriestessTests
{
    private static readonly string CARD_KEY = "ii-the-high-priestess-b";

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task OnSelf(int roll)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PreventDamage(roll)
                .PlayLootCard(CARD_KEY)
                    .Choose.Me()
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(roll)
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner()
            .HasNoDamage()
            .HasNoDamagePreventors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task OnOpponent(int roll)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PreventDamage(roll)
                .PlayLootCard(CARD_KEY)
                    .Choose.Player(opponentIdx)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();
        
        var opponent = new ProgrammedPlayerControllerBuilder("judas-b")
            .ConfigActions()
                .PreventDamage(roll)
                .AutoPass()
                .Done()
            .Build();

        var roller = new ProgrammedRollerBuilder()
            .Then(roll)
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            opponent,
        ];

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner();
        match.AssertPlayer(opponentIdx)
            .HasNoDamage()
            .HasNoDamagePreventors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    // [InlineData(4)] // TODO add back
    // [InlineData(5)]
    // [InlineData(6)]
    public async Task OnMonster(int roll)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialMonsterSlots(1)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .ConfigMonsterDeck().AddMonster("monstro-b").Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PreventDamage(roll)
                .PlayLootCard(CARD_KEY)
                    .Choose.MonsterInSlot(0)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();
        
        var roller = new ProgrammedRollerBuilder()
            .Then(roll)
            .Build();

        List<ProgrammedPlayerController> players = [
            mainPlayer,
            ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
        ];

        var match = new TestMatch(config, mainPlayerIdx, roller: roller);
        await match.AddPlayers(players);

        // Act
        await match.Run();

        // Assert
        match.AssertPlayer(mainPlayerIdx)
            .IsWinner();
        match.AssertMonsterInSlot(0)
            .HasDamage(roll);
            // .HasNoDamage()
            // .HasNoDamagePreventors();
    }

    // [Theory]
    // [InlineData(1)]
    // [InlineData(2)]
    // [InlineData(3)]
    // [InlineData(4)]
    // [InlineData(5)]
    // [InlineData(6)]
    // public async Task OnMonster()
    // {
    //     // Arrange
    //     var mainPlayerIdx = 0;
    //     var CARD_KEY = "bomb-b";
    //     var opponentIdx = 1 - mainPlayerIdx;
    //     var lootDeckSize = 10;
    //     var config = new MatchConfigBuilder()
    //         .InitialCoins(0)
    //         .InitialLoot(0)
    //         .InitialMonsterSlots(1)
    //         .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
    //         .Build();

    //     var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
    //         .ConfigActions()
    //             .AssertIsCurrentPlayer()
    //             .PlayLootCard(CARD_KEY)
    //                 .Choose.MonsterInSlot(0)
    //             .AutoPassUntilEmptyStack()
    //             .SetWinner()
    //             .Done()
    //         .Build();

    //     var opponent = 

    //     List<ProgrammedPlayerController> players = [
    //         mainPlayer,
    //         ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
    //     ];

    //     var match = new TestMatch(config, mainPlayerIdx);
    //     await match.AddPlayers(players);

    //     // Act
    //     await match.Run();

    //     // Assert
    //     match.AssertPlayer(mainPlayerIdx)
    //         .IsWinner();
    //     match.AssertMonsterInSlot(0)
    //         .HasDamage(1);
    // }
}
namespace FSCore.Tests.Matches.Cards.Loot;

public class TheTowerTests
{
    private static readonly string CARD_KEY = "xvi-the-tower-b";

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(5, 2)]
    [InlineData(6, 2)]
    public async Task PlayerDamageRolls(int roll, int damage)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var opponentIdx = 1 - mainPlayerIdx;
        var lootDeckSize = 10;
        var itemKey = "breakfast-b";
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialTreasureSlots(0)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(itemKey)
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
                .AutoPassUntilEmptyStack()
                .SetWinner()
                .Done()
            .Build();

        var opponent = new ProgrammedPlayerControllerBuilder("isaac-b")
            .HasItemAtStart(itemKey)
            .ConfigActions()
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
            .IsWinner()
            .HasDamage(damage);
        match.AssertPlayer(opponentIdx)
            .HasDamage(damage);
    }
    
    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public async Task MonsterDamageRolls(int roll)
    {
        // Arrange
        var mainPlayerIdx = 0;
        var lootDeckSize = 10;
        int monsterSlots = 2;
        var config = new MatchConfigBuilder()
            .InitialCoins(0)
            .InitialLoot(0)
            .InitialTreasureSlots(0)
            .InitialMonsterSlots(monsterSlots)
            .ConfigLootDeck().Add(CARD_KEY, lootDeckSize).Done()
            .ConfigMonsterDeck().AddMonster("clotty-b").AddMonster("fatty-b").Done()
            .Build();

        var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
            .ConfigActions()
                .AssertIsCurrentPlayer()
                .PlayLootCard(CARD_KEY)
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
        for (int i = 0; i < monsterSlots; ++i) {
            match.AssertMonsterInSlot(i)
                .HasDamage(1);
        }
    }

    
}
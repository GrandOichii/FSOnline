// namespace FSCore.Tests.Matches.Treasures;

// public class BabyHauntTests
// {
//     private static readonly string CARD_KEY = "golden-razor-blade-b";

//     [Fact]
//     public async Task Base()
//     {
//         // Arrange
//         var monsterKey = "monstro-b";
//         var mainPlayerIdx = 0;
//         var opponentIdx = 1 - mainPlayerIdx;
//         var config = new MatchConfigBuilder()
//             .InitialCoins(0)
//             .InitialLoot(0)
//             .LootStepLootAmount(0)
//             .LootPlay(0)
//             .InitialTreasureSlots(0)
//             .InitialMonsterSlots(1)
//             .DisableDeathPenalty()
//             .ConfigMonsterDeck().AddMonster(monsterKey).Done()
//             .Build();

//         var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
//             .HasItemAtStart(CARD_KEY)
//             .ConfigActions()
//                 .DeclareAttack(0)
//                 .AutoPassUntilEmptyStack()
//                 .SetWinner()
//                 .Done()
//             .Build();

//         var roller = new ProgrammedRollerBuilder()
//             .ThenNTimes(4, 2)
//             .Build();

//         List<ProgrammedPlayerController> players = [
//             mainPlayer,
//             ProgrammedPlayerControllers.AutoPassPlayerController("judas-b"),
//         ];

//         var match = new TestMatch(config, mainPlayerIdx, roller: roller);
//         await match.AddPlayers(players);

//         // Act
//         await match.Run();

//         // Assert
//         match.AssertPlayer(mainPlayerIdx)
//             .IsWinner()
//             .DoesntHaveSoulCard(monsterKey);
//         match.AssertPlayer(opponentIdx)
//             .HasItem(CARD_KEY);
//     }
// }
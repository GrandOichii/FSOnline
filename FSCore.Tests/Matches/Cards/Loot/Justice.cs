// TODO uncomment after implementing
// namespace FSCore.Tests.Matches.Cards.Loot;

// public class JusticeTests
// {
//     private static readonly string CARD_KEY = "viii-justice-b";

//     [Theory]
//     [InlineData(1, 1)]
//     [InlineData(5, 0)]
//     [InlineData(0, 5)]
//     public async Task OnSelf(int coins, int loot)
//     {
//         // Arrange
//         var mainPlayerIdx = 0;
//         var opponentIdx = 1 - mainPlayerIdx;
//         var lootDeckSize = 10;
//         var config = new MatchConfigBuilder()
//             .InitialCoins(0)
//             .InitialLoot(0)
//             .LootStepLootAmount(0)
//             .ConfigLootDeck(d => d.Add(CARD_KEY, lootDeckSize))
//             .Build();

//         var mainPlayer = new ProgrammedPlayerControllerBuilder("isaac-b")
//             .ConfigActions()
//                 .AssertIsCurrentPlayer()
//                 .Pass()
//                 .AutoPassUntilMyTurn()
//                 .LootCards(1)
//                 .PlayLootCard(CARD_KEY)
//                     .Choose.Player(opponentIdx)
//                 .AutoPassUntilEmptyStack()
//                 .SetWinner()
//                 .Done()
//             .Build();
    
//         var opponent = new ProgrammedPlayerControllerBuilder("judas-b")
//             .ConfigActions()
//                 .AutoPassUntilMyTurn()
//                 .GainCoins(coins)
//                 .LootCards(loot)
//                 .AutoPass()
//                 .Done()
//             .Build();

//         List<ProgrammedPlayerController> players = [
//             mainPlayer,
//             opponent
//         ];

//         var match = new TestMatch(config, mainPlayerIdx);
//         await match.AddPlayers(players);

//         // Act
//         await match.Run();

//         // Assert
//         match.AssertPlayer(mainPlayerIdx)
//             .IsWinner()
//             .HasNoDamage()
//             .HasNoDamagePreventors();
//     }
// }
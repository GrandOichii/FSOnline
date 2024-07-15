using FSCore.Matches;
using FSCore.Matches.Players.Controllers;

namespace FSTester;

public class ConsolePlayerController : IPlayerController
{
    public Task Cleanup(Match match, int playerIdx)
    {
        throw new NotImplementedException();
    }

    public Task Setup(Match match, int playerIdx)
    {
        throw new NotImplementedException();
    }
}

public class Program {
    public static void Main(string[] args) {
        var config = new MatchConfig() {
            PlayerCount = 2,
            CoinPool = 100,
            InitialTreasureSlots = 2,
            InitialMonsterSlots = 2,
            InitialRoomSlots = 2,
            BonusSoulCount = 3,
            InitialDealLoot = 3,
            InitialDealCoins = 3,
            SoulsToWin = 4,
            ForceInclude3PlusCards = true,
            RandomFirstPlayer = true
        };

        var c1 = new ConsolePlayerController();
        var c2 = c1;

        // var match = new Match(config, 0, );
    }
}

namespace FSCore.Tests.Setup;

public class TestMatch {
    private readonly Match _match;
    private readonly FileCardMaster _cm;

    public TestMatch(
        MatchConfig config,
        int firstPlayerIdx,
        int seed = 0,
        IRoller? roller = null
    )
    {
        _cm = new();
        // TODO
        _cm.Load("../../../../cards/b");

        _match = new(
            config,
            seed, // TODO change
            _cm,
            File.ReadAllText("../../../../core.lua"),
            roller
        )
        {
            CurPlayerIdx = firstPlayerIdx
        };
    }

    public async Task AddPlayers(List<ProgrammedPlayerController> players)
    {
        for (int i = 0; i < players.Count; ++i)
        {
            await _match.AddPlayer($"pp{i + 1}", players[i], players[i].Character);
        }
    }

    public async Task Run()
    {
        await _match.Run();
    }

    #region Asserts

    public void AssertIsWinner(int playerIdx) {
        Assert.Equal(_match.WinnerIdx, playerIdx);
    }

    public void AssertHasCoins(int playerIdx, int amount) {
        Assert.Equal(_match.GetPlayer(playerIdx).Coins, amount);
    }

    public void AssertCoinsInBank(int amount) {
        Assert.Equal(_match.CoinPool, amount);
    }

    #endregion
}
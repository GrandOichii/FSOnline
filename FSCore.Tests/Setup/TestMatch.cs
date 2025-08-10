
namespace FSCore.Tests.Setup;

public class TestMatch
{
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

    public void AssertIsWinner(int playerIdx)
    {
        Assert.Equal(playerIdx, _match.WinnerIdx);
    }

    public void AssertHasHealth(int playerIdx, int amount)
    {
        Assert.Equal(amount, _match.GetPlayer(playerIdx).Stats.GetCurrentHealth());
    }

    public void AssertHasCoins(int playerIdx, int amount)
    {
        Assert.Equal(amount, _match.GetPlayer(playerIdx).Coins);
    }

    public void AssertCoinsInBank(int amount)
    {
        Assert.Equal(amount, _match.CoinPool);
    }

    public void AssertCardsInHand(int playerIdx, int amount)
    {
        Assert.Equal(amount, _match.GetPlayer(playerIdx).Hand.Count);
    }

    public void AssertCardsInLootDeck(int amount)
    {
        Assert.Equal(amount, _match.LootDeck.Size);
    }

    public void AssertCardsInLootDiscard(int amount)
    {
        Assert.Equal(amount, _match.LootDeck.Discard!.Count);
    }

    public void AssertHasItemCount(int playerIdx, int amount)
    {
        Assert.Equal(amount, _match.GetPlayer(playerIdx).Items.Count);
    }
    
    public void AssertHasItem(int playerIdx, string key)
    {
        Assert.NotNull(_match.GetPlayer(playerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == key));
    }



    #endregion
}
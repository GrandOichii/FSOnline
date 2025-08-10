
namespace FSCore.Tests.Setup;

public class TestMatch
{
    public Match Match { get; }
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
        _cm.Load("../../../../cards/b2");

        Match = new(
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
            await Match.AddPlayer($"pp{i + 1}", players[i], players[i].Character);
        }
    }

    public async Task Run()
    {
        await Match.Run();
    }

    #region Asserts

    public void AssertIsWinner(int playerIdx)
    {
        Match.WinnerIdx.ShouldBe(playerIdx);
    }

    public void AssertHasHealth(int playerIdx, int amount)

    {
        Match.GetPlayer(playerIdx).Stats.GetCurrentHealth().ShouldBe(amount);
    }

    public void AssertHasCoins(int playerIdx, int amount)
    {
        Match.GetPlayer(playerIdx).Coins.ShouldBe(amount);
    }

    public void AssertCoinsInBank(int amount)
    {
        Match.CoinPool.ShouldBe(amount);
    }

    public void AssertCardsInHand(int playerIdx, int amount)
    {
        Match.GetPlayer(playerIdx).Hand.Count.ShouldBe(amount);
    }

    public void AssertCardsInLootDeck(int amount)
    {
        Match.LootDeck.Size.ShouldBe(amount);
    }

    public void AssertCardsInLootDiscard(int amount)
    {
        Match.LootDeck.Discard!.Count.ShouldBe(amount);
    }

    public void AssertHasItemCount(int playerIdx, int amount)
    {
        Match.GetPlayer(playerIdx).Items.Count.ShouldBe(amount);
    }
    
    public void AssertHasItem(int playerIdx, string key)
    {
        Match.GetPlayer(playerIdx).Items.FirstOrDefault(i => i.Card.Template.Key == key).ShouldNotBeNull();
    }

    #endregion
}
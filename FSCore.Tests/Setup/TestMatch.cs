
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

    public void AssertHasLootPlays(int playerIdx, int amount)
    {
        var p = Match.GetPlayer(playerIdx);
        (p.State.LootPlaysForTurn - p.LootPlayed).ShouldBe(amount);
    }

    public void AssertPlayerHasHealth(int playerIdx, int amount)
    {
        Match.GetPlayer(playerIdx).Stats.GetCurrentHealth().ShouldBe(amount);
    }

    public void AssertPlayerHasDamage(int playerIdx, int amount)
    {
        Match.GetPlayer(playerIdx).Stats.Damage.ShouldBe(amount);
    }

    public void AssertHasAttack(int playerIdx, int amount)
    {
        Match.GetPlayer(playerIdx).Stats.State.Attack.ShouldBe(amount);
    }

    public void AssertPlayerHasAttackOpportunities(int playerIdx, int amount)
    {
        Match.GetPlayer(playerIdx).AttackOpportunities.ShouldBe(amount);
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

    public void AssertHasSoulCard(int playerIdx, string key)
    {
        Match.GetPlayer(playerIdx).Souls.FirstOrDefault(s => s.Original.Template.Key == key).ShouldNotBeNull();
    }

    public PlayerAssertions AssertPlayer(int playerIdx)
    {
        return new(Match.GetPlayer(playerIdx));
    }

    #endregion
}


public class PlayerAssertions(Player player)
{
    private Match Match => player.Match;

    public PlayerAssertions IsWinner()
    {
        Match.WinnerIdx.ShouldBe(player.Idx);
        return this;
    }

    public PlayerAssertions IsDead()
    {
        player.Stats.IsDead.ShouldBeTrue();
        return this;
    }

    public PlayerAssertions IsCurrentPlayer()
    {
        Match.CurPlayerIdx.ShouldBe(player.Idx);
        return this;
    }
    
}
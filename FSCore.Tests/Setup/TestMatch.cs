
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

    public void AssertCoinsInBank(int amount)
    {
        Match.CoinPool.ShouldBe(amount);
    }

    public void AssertCardsInLootDeck(int amount)
    {
        Match.LootDeck.Size.ShouldBe(amount);
    }

    public void AssertCardsInLootDiscard(int amount)
    {
        Match.LootDeck.Discard!.Count.ShouldBe(amount);
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

    public PlayerAssertions HasLootPlays(int amount)
    {
        (player.State.LootPlaysForTurn - player.LootPlayed).ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasHealth(int amount)
    {
        player.Stats.GetCurrentHealth().ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasDamage(int amount)
    {
        player.Stats.Damage.ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasAttack(int amount)
    {
        player.Stats.State.Attack.ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasAttackOpportunities(int amount)
    {
        player.AttackOpportunities.ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasCoins(int amount)
    {
        player.Coins.ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasCardsInHand(int amount)
    {
        player.Hand.Count.ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasItemCount(int amount)
    {
        player.Items.Count.ShouldBe(amount);
        return this;
    }

    public PlayerAssertions HasItem(string key)
    {
        player.Items.FirstOrDefault(i => i.Card.Template.Key == key).ShouldNotBeNull();
        return this;
    }

    public PlayerAssertions HasSoulCard(string key)
    {
        player.Souls.FirstOrDefault(s => s.Original.Template.Key == key).ShouldNotBeNull();
        return this;
    }
}
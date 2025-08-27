namespace FSCore.Tests.Matches.Treasures;

public class TheRelicTests
{
    private static readonly string CARD_KEY = "the-relic-b";

    [Fact]
    public async Task LootRoll()
    {
        await Common.SetupTreasureRollTest(
            CARD_KEY,
            1,
            (match, mainPlayerIdx) =>
            {
                match.AssertCardsInHand(mainPlayerIdx, 1);
            }
        );
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    public async Task NoLootRoll(int roll)
    {
        await Common.SetupTreasureRollTest(
            CARD_KEY,
            roll,
            (match, mainPlayerIdx) =>
            {
                match.AssertCardsInHand(mainPlayerIdx, 0);
            }
        );
    }
}
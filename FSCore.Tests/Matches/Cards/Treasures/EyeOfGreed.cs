namespace FSCore.Tests.Matches.Treasures;

public class EyeOfGreedTests
{
    private static readonly string CARD_KEY = "eye-of-greed-b";

    [Fact]
    public async Task CoinRoll()
    {
        await Common.SetupTreasureRollTest(
            CARD_KEY,
            5,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(3);
            }
        );
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(6)]
    public async Task NoCoinRoll(int roll)
    {
        await Common.SetupTreasureRollTest(
            CARD_KEY,
            roll,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(0);
            }
        );
    }
}
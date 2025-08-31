namespace FSCore.Tests.Matches.Monsters;

public class BasicMonsterTests
{
    [Fact]
    public async Task Squirt()
    {
        await Common.SetupRewardsTest(
            "squirt-b",
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(1);
            }
        );
    }

    [Fact]
    public async Task Fly()
    {
        await Common.SetupRewardsTest(
            "fly-b",
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(1);
            }
        );
    }

    [Fact]
    public async Task Fatty()
    {
        await Common.SetupRewardsTest(
            "fatty-b",
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(1);
            }
        );
    }

    [Fact]
    public async Task Spider()
    {
        await Common.SetupRewardsTest(
            "spider-b",
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(1);
            }
        );
    }

    [Fact]
    public async Task FatBat()
    {
        await Common.SetupRewardsTest(
            "fat-bat-b",
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasItemCount(2);
            }
        );
    }

    [Fact]
    public async Task Gurdy()
    {
        var key = "gurdy-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(7)
                    .HasSoulCard(key);
            }
        );
    }

    // [Fact]
    // public async Task CodWorm()
    // {
    //     var key = "cod-worm-b";
    //     await Common.SetupRewardsTest(
    //         key,
    //         (match, mainPlayerIdx) =>
    //         {
    //             match.AssertPlayer(mainPlayerIdx)
    //  .HasCoins(3);
    //         }
    //     );
    // }

    [Fact]
    public async Task Dip()
    {
        var key = "dip-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(1);
            }
        );
    }

    [Fact]
    public async Task Trite()
    {
        var key = "trite-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(2);
            }
        );
    }

    [Fact]
    public async Task ConjoinedFatty()
    {
        var key = "conjoined-fatty-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(2);
            }
        );
    }

    [Fact]
    public async Task Clotty()
    {
        var key = "clotty-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(4);
            }
        );
    }

    [Fact]
    public async Task PaleFatty()
    {
        var key = "pale-fatty-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(6);
            }
        );
    }


    [Fact]
    public async Task Pooter()
    {
        var key = "pooter-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(1);
            }
        );
    }

    [Fact]
    public async Task RedHost()
    {
        var key = "red-host-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(5);
            }
        );
    }

    [Fact]
    public async Task Leech()
    {
        var key = "leech-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(1);
            }
        );
    }

    [Fact]
    public async Task Monstro()
    {
        var key = "monstro-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCoins(6)
                    .HasSoulCard(key);
            }
        );
    }

    [Fact]
    public async Task LittleHorn()
    {
        var key = "little-horn-b";
        await Common.SetupRewardsTest(
            key,
            (match, mainPlayerIdx) =>
            {
                match.AssertPlayer(mainPlayerIdx)
                    .HasCardsInHand(2)
                    .HasSoulCard(key);
            }
        );
    }
}
                // match.AssertPlayer(mainPlayerIdx)
                //  .HasCoins(1);
                // match.AssertPlayer(mainPlayerIdx)
                //  .HasCardsInHand(1);
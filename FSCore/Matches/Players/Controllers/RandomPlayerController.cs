using FSCore.Matches;
using FSCore.Matches.Players.Controllers;

namespace FSCore.Matches.Players.Controllers;

public class RandomPlayerController : IPlayerController
{
    private readonly Random _rnd;
    private readonly int _delay;

    public RandomPlayerController(int seed, int delayMS = 0) {
        _rnd = new Random(seed);
        _delay = delayMS;
    }

    private T GetRandom<T>(IEnumerable<T> items) {
        var result = 0;
        foreach (var _ in items) ++result;
        return items.ElementAt(_rnd.Next() % result);
    }

    public async Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public async Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public Task CleanUp(Match match, int playerIdx)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
    }

    public async Task<string> PromptAction(Match match, int playerI, IEnumerable<string> options)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public async Task Setup(Match match, int playerI)
    {
        await Task.Delay(_delay);
    }

    public async Task Update(Match match, int playerI) { }

    public async Task<string> ChooseStackEffect(Match match, int playerIdx, List<string> options, string hint)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public async Task<int> ChooseCardInHand(Match match, int playerIdx, List<int> options, string hint)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public async Task<string> ChooseItem(Match match, int playerIdx, List<string> options, string hint)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public async Task<int> ChooseItemToPurchase(Match match, int playerIdx, List<int> options)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public async Task<int> ChooseMonsterToAttack(Match match, int playerIdx, List<int> options)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }

    public async Task<(TargetType, string)> ChooseMonsterOrPlayer(Match match, int playerIdx, List<string> ipids, List<int> indicies, string hint)
    {
        await Task.Delay(_delay);
        var options = ipids;
        options.AddRange(indicies.Select(i => i.ToString()));
        var result = GetRandom(options);
        return (
            ipids.Contains(result) ? TargetType.ITEM : TargetType.PLAYER,
            result
        );
    }

    public async Task<DeckType> ChooseDeck(Match match, int playerIdx, List<DeckType> options, string hint)
    {
        await Task.Delay(_delay);
        return GetRandom(options);
    }
}

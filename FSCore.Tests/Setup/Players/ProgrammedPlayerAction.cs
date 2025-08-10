namespace FSCore.Tests.Setup.Players;

public interface IProgrammedPlayerAction {
    public static readonly string NEXT_ACTION = "";

    public Task<(string, bool)> Do(Match match, int playerIdx);

}

public class AutoPassAction : IProgrammedPlayerAction {
    public static AutoPassAction Instance { get; } = new();

    private AutoPassAction() {}

    public Task<(string, bool)> Do(Match match, int playerIdx) {
        return Task.FromResult(
            (new PassAction().ActionWord(), false)
        );
    }
}

public class AssertIsCurrentPlayerAction : IProgrammedPlayerAction {
    public static AssertIsCurrentPlayerAction Instance { get; } = new();

    private AssertIsCurrentPlayerAction() {}

    public Task<(string, bool)> Do(Match match, int playerIdx) {
        
        // TODO do an actual assert
        if (playerIdx == match.CurPlayerIdx)
            return Task.FromResult((IProgrammedPlayerAction.NEXT_ACTION, true));
        throw new Exception($"Failed {nameof(AssertIsCurrentPlayerAction)} assertion");
    }
}

public class AssertHasCardsInHand(int amount) : IProgrammedPlayerAction
{
    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        // TODO do an actual assert
        if (player.Hand.Count == amount)
            return Task.FromResult((IProgrammedPlayerAction.NEXT_ACTION, true));
        throw new Exception($"Failed {nameof(AssertIsCurrentPlayerAction)} assertion");
    }
}

public class SetWinnerAction : IProgrammedPlayerAction
{
    public static SetWinnerAction Instance { get; } = new();

    private SetWinnerAction() { }

    public async Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        player.State.AdditionalSoulCount = match.Config.SoulsToWin;
        await match.CheckWinners();
        return (new PassAction().ActionWord(), true);
    }
}

public class AutoPassUntilEmptyStackAction : IProgrammedPlayerAction
{
    public static AutoPassUntilEmptyStackAction Instance { get; } = new();

    private AutoPassUntilEmptyStackAction() { }

    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        if (match.Stack.Effects.Count > 0)
        {
            return Task.FromResult((new PassAction().ActionWord(), false));
        }
        return Task.FromResult((IProgrammedPlayerAction.NEXT_ACTION, true));
    }
}

public class PlayLootCardAction(string key) : IProgrammedPlayerAction
{
    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        var card = GetHandMatchCard(player, key)
            ?? throw new Exception($"Player {player.Name} doesn't have {key} in their hand to play");

        return Task.FromResult(
            ($"{new PlayLootAction().ActionWord()} {card.Card.ID}", true)
        );
    }

    private static HandMatchCard? GetHandMatchCard(Player player, string key) {
        return player.Hand.FirstOrDefault(c => c.Card.Template.Key == key);
    }
}

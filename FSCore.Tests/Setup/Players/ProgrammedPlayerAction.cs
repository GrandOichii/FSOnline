namespace FSCore.Tests.Setup.Players;

public interface IProgrammedPlayerAction {
    public static readonly string NEXT_ACTION = "";

    public Task<(string, bool)> Do(Match match, int playerIdx);

}

public class AutoPassPPAction : IProgrammedPlayerAction {
    public static AutoPassPPAction Instance { get; } = new();

    private AutoPassPPAction() {}

    public Task<(string, bool)> Do(Match match, int playerIdx) {
        return Task.FromResult(
            (new PassAction().ActionWord(), false)
        );
    }
}

public class AssertIsCurrentPlayerPPAction : IProgrammedPlayerAction {
    public static AssertIsCurrentPlayerPPAction Instance { get; } = new();

    private AssertIsCurrentPlayerPPAction() {}

    public Task<(string, bool)> Do(Match match, int playerIdx) {
        
        // TODO do an actual assert
        if (playerIdx == match.CurPlayerIdx)
            return Task.FromResult((IProgrammedPlayerAction.NEXT_ACTION, true));
        throw new Exception($"Failed {nameof(AssertIsCurrentPlayerPPAction)} assertion");
    }
}

public class AssertHasCardsInHandPPAction(int amount) : IProgrammedPlayerAction
{
    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        // TODO do an actual assert
        if (player.Hand.Count == amount)
            return Task.FromResult((IProgrammedPlayerAction.NEXT_ACTION, true));
        throw new Exception($"Failed {nameof(AssertIsCurrentPlayerPPAction)} assertion");
    }
}

public class SetWinnerPPAction : IProgrammedPlayerAction
{
    public static SetWinnerPPAction Instance { get; } = new();

    private SetWinnerPPAction() { }

    public async Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        player.State.AdditionalSoulCount = match.Config.SoulsToWin;
        await match.CheckWinners();
        return (new PassAction().ActionWord(), true);
    }
}

public class AutoPassUntilEmptyStackPPAction : IProgrammedPlayerAction
{
    public static AutoPassUntilEmptyStackPPAction Instance { get; } = new();

    private AutoPassUntilEmptyStackPPAction() { }

    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        if (match.Stack.Effects.Count > 0)
        {
            return Task.FromResult((new PassAction().ActionWord(), false));
        }
        return Task.FromResult((IProgrammedPlayerAction.NEXT_ACTION, true));
    }
}

public class AutoPassUntilMyTurnPPAction : IProgrammedPlayerAction
{
    public static AutoPassUntilMyTurnPPAction Instance { get; } = new();

    private AutoPassUntilMyTurnPPAction() { }

    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        if (match.CurPlayerIdx != playerIdx)
        {
            return Task.FromResult((new PassAction().ActionWord(), false));
        }
        return Task.FromResult((IProgrammedPlayerAction.NEXT_ACTION, true));
    }
}

public class PassPPAction : IProgrammedPlayerAction
{
    public static PassPPAction Instance { get; } = new();

    private PassPPAction() { }

    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        return Task.FromResult((new PassAction().ActionWord(), true));
    }
}

public class PlayLootCardPPAction(string key) : IProgrammedPlayerAction
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

public class RemoveFromPlayPPAction(string key) : IProgrammedPlayerAction
{
    public async Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        var card = GetItem(player, key)
            ?? throw new Exception($"Player {player.Name} doesn't have {key} item in play");

        await player.RemoveFromPlay(card);
        return (IProgrammedPlayerAction.NEXT_ACTION, true);
    }

    private static OwnedInPlayMatchCard? GetItem(Player player, string key) {
        return player.Items.FirstOrDefault(c => c.Card.Template.Key == key);
    }
}

public class GainTreasurePPAction(int amount) : IProgrammedPlayerAction
{
    public async Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        await player.GainTreasure(amount);
        return (IProgrammedPlayerAction.NEXT_ACTION, true);
    }
}

public class ActivateOwnedItemPPAction(string key, int abilityIdx) : IProgrammedPlayerAction
{
    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        var item = GetItem(player, key)
            ?? throw new Exception($"Player {player.Name} doesn't have {key} item in play");
        return Task.FromResult(
            ($"{new ActivateAction().ActionWord()} {item.IPID} {abilityIdx}", true)
        );
    }

    // TODO duplicate
    private static OwnedInPlayMatchCard? GetItem(Player player, string key)
    {
        return player.Items.FirstOrDefault(c => c.Card.Template.Key == key);
    }
}

public class ActivateCharacterPPAction(int abilityIdx) : IProgrammedPlayerAction
{
    public Task<(string, bool)> Do(Match match, int playerIdx)
    {
        var player = match.GetPlayer(playerIdx);
        return Task.FromResult(
            ($"{new ActivateAction().ActionWord()} {player.Character.IPID} {abilityIdx}", true)
        );
    }
}

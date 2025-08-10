namespace FSCore.Tests.Setup.Players;

public class ProgrammedPlayerControllerBuilder {
    private readonly ProgrammedPlayerActionsBuilder _actions;
    public ProgrammedPlayerController Result { get; } = new();

    public ProgrammedPlayerControllerBuilder(string character) {
        _actions = new(this);
        Result.Character = character;
    }

    public ProgrammedPlayerActionsBuilder ConfigActions() => _actions;

    public ProgrammedPlayerController Build() => Result;
}

public class ProgrammedPlayerActionsBuilder(ProgrammedPlayerControllerBuilder parent)
{

    public ProgrammedPlayerControllerBuilder Done() => parent;

    public ProgrammedPlayerActionsBuilder AssertIsCurrentPlayer()
    {
        parent.Result.Actions.Enqueue(AssertIsCurrentPlayerAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder AssertHasCardsInHand(int amount)
    {
        parent.Result.Actions.Enqueue(
            new AssertHasCardsInHand(amount)
        );
        return this;
    }

    public ProgrammedPlayerActionsBuilder PlayLootCard(string cardKey)
    {
        parent.Result.Actions.Enqueue(
            new PlayLootCardAction(cardKey)
        );
        return this;
    }

    public ProgrammedPlayerActionsBuilder AutoPassUntilEmptyStack()
    {
        parent.Result.Actions.Enqueue(AutoPassUntilEmptyStackAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder SetWinner()
    {
        parent.Result.Actions.Enqueue(SetWinnerAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder AutoPass()
    {
        parent.Result.Actions.Enqueue(AutoPassAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder RemoveItem(string cardKey)
    {
        parent.Result.Actions.Enqueue(new RemoveFromPlayAction(cardKey));
        return this;
    }
}

public class ProgrammedPlayerController : IPlayerController {
    public string Character { get; set; } = "";

    public Queue<IProgrammedPlayerAction> Actions { get; } = new();

    public Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task CleanUp(Match match, int playerIdx)
    {
        // throw new NotImplementedException();
        return Task.CompletedTask;
    }

    public async Task<string> PromptAction(Match match, int playerIdx, IEnumerable<string> options)
    {
        var result = IProgrammedPlayerAction.NEXT_ACTION;
        // var count = 0;
        // var max = 0;
        while (result == IProgrammedPlayerAction.NEXT_ACTION)
        {
            if (!Actions.TryPeek(out var action))
                throw new Exception("No actions left in queue!");
            bool next;
            (result, next) = await action.Do(match, playerIdx);
            if (next) Actions.Dequeue();
        }

        if (!options.Contains(result))
        {
            throw new Exception($"Received action \"{result}\", which is not a valid action! (expected: \"{string.Join(", ", options)}\")");
        }

        return result;
    }

    public Task Setup(Match match, int playerI)
    {
        // throw new NotImplementedException();
        return Task.CompletedTask;
    }

    public Task Update(Match match, int playerI) { return Task.CompletedTask; }

    public Task<string> ChooseStackEffect(Match match, int playerIdx, List<string> options, string hint)
    {
        throw new NotImplementedException();
        // return Task.CompletedTask;
    }

    public Task<int> ChooseCardInHand(Match match, int playerIdx, List<int> options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<string> ChooseItem(Match match, int playerIdx, List<string> options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<int> ChooseItemToPurchase(Match match, int playerIdx, List<int> options)
    {
        throw new NotImplementedException();
    }

    public Task<int> ChooseMonsterToAttack(Match match, int playerIdx, List<int> options)
    {
        throw new NotImplementedException();
    }

    public Task<(TargetType, string)> ChooseMonsterOrPlayer(Match match, int playerIdx, List<string> ipids, List<int> indicies, string hint)
    {
        throw new NotImplementedException();
    }

    public Task<DeckType> ChooseDeck(Match match, int playerIdx, List<DeckType> options, string hint)
    {
        throw new NotImplementedException();
    }

}

public static class ProgrammedPlayerControllers {
    public static ProgrammedPlayerController AutoPassPlayerController(string characterKey) => new ProgrammedPlayerControllerBuilder(characterKey)
        .ConfigActions()
            .AutoPass()
            .Done()
        .Build();
}
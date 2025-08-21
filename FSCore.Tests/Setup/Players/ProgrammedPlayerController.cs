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

public class ProgrammedPlayerActionsBuilder
{
    public ProgrammedPlayerControllerBuilder Parent { get; }
    public ProgrammedPlayerControllerBuilder Done() => Parent;

    public ChoiceBuilder Choose { get; }

    public ProgrammedPlayerActionsBuilder(ProgrammedPlayerControllerBuilder parent)
    {
        Parent = parent;
        Choose = new(this);
    }

    public ProgrammedPlayerActionsBuilder AssertIsCurrentPlayer()
    {
        Parent.Result.Actions.Enqueue(AssertIsCurrentPlayerPPAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder GainTreasure(int amount)
    {
        Parent.Result.Actions.Enqueue(new GainTreasurePPAction(amount));
        return this;
    }

    public ProgrammedPlayerActionsBuilder Pass()
    {
        Parent.Result.Actions.Enqueue(PassPPAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder ActivateOwnedItem(string key, int abilityIdx)
    {
        Parent.Result.Actions.Enqueue(new ActivateOwnedItemPPAction(key, abilityIdx));
        return this;
    }

    public ProgrammedPlayerActionsBuilder ActivateCharacter(int abilityIdx = 0)
    {
        Parent.Result.Actions.Enqueue(new ActivateCharacterPPAction(abilityIdx));
        return this;
    }

    public ProgrammedPlayerActionsBuilder AssertHasCardsInHand(int amount)
    {
        Parent.Result.Actions.Enqueue(
            new AssertHasCardsInHandPPAction(amount)
        );
        return this;
    }

    public ProgrammedPlayerActionsBuilder PlayLootCard(string cardKey)
    {
        Parent.Result.Actions.Enqueue(
            new PlayLootCardPPAction(cardKey)
        );
        return this;
    }

    public ProgrammedPlayerActionsBuilder AutoPassUntilEmptyStack()
    {
        Parent.Result.Actions.Enqueue(AutoPassUntilEmptyStackPPAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder AutoPassUntilMyTurn()
    {
        Parent.Result.Actions.Enqueue(AutoPassUntilMyTurnPPAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder SetWinner()
    {
        Parent.Result.Actions.Enqueue(SetWinnerPPAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder AutoPass()
    {
        Parent.Result.Actions.Enqueue(AutoPassPPAction.Instance);
        return this;
    }

    public ProgrammedPlayerActionsBuilder RemoveItem(string cardKey)
    {
        Parent.Result.Actions.Enqueue(new RemoveFromPlayPPAction(cardKey));
        return this;
    }

    public ProgrammedPlayerActionsBuilder PreventDamage(int amount = 1)
    {
        Parent.Result.Actions.Enqueue(new PreventDamagePPAction(amount));
        return this;
    }
}

public class ChoiceBuilder(ProgrammedPlayerActionsBuilder parent)
{
    public ProgrammedPlayerActionsBuilder CardInHand(int idx)
    {
        parent.Parent.Result.HandCardChoiceQueue.Enqueue(idx);
        return parent;
    }

    public ProgrammedPlayerActionsBuilder Me()
    {
        parent.Parent.Result.PlayerChoiceQueue.Enqueue(-1);
        return parent;
    }

    public ProgrammedPlayerActionsBuilder Option(int optionIdx)
    {
        parent.Parent.Result.OptionsQueue.Enqueue(optionIdx);
        return parent;
    }
}

public class ProgrammedPlayerController : IPlayerController
{
    public string Character { get; set; } = "";

    public Queue<IProgrammedPlayerAction> Actions { get; } = new();
    public Queue<int> HandCardChoiceQueue { get; } = new();
    public Queue<int> PlayerChoiceQueue { get; } = new();
    public Queue<int> OptionsQueue { get; } = new();


    public Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint)
    {
        if (OptionsQueue.TryDequeue(out var result))
        {
            return Task.FromResult(options[result]);
        }

        throw new Exception("Choose string queue is empty");
    }

    public Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint)
    {
        if (PlayerChoiceQueue.TryDequeue(out var result))
        {
            if (result == -1)
                result = playerIdx;
            return Task.FromResult(result);
        }

        throw new Exception("Player choice queue is empty");
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
        if (HandCardChoiceQueue.TryDequeue(out var result))
        {
            return Task.FromResult(result);
        }

        throw new Exception("Hand card choice queue is empty");
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
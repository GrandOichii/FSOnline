namespace FSCore.Tests.Setup.Players;

public class ProgrammedPlayerControllerBuilder {
    private readonly ProgrammedPlayerActionsBuilder _actions;
    public ProgrammedPlayerController Result { get; } = new();

    public ProgrammedPlayerControllerBuilder(string character) {
        _actions = new(this);
        Result.Character = character;
    }

    public ProgrammedPlayerControllerBuilder HasItemAtStart(string itemKey)
    {
        Result.Setups.Enqueue(new GainItemPPSetup(itemKey));
        return this;
    }

    public ProgrammedPlayerActionsBuilder ConfigActions() => _actions;

    public ProgrammedPlayerController Build() {

        return Result;
    }
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

    public ProgrammedPlayerActionsBuilder ActivateOwnedItem(string key, int abilityIdx=0)
    {
        Parent.Result.Actions.Enqueue(new ActivateOwnedItemPPAction(key, abilityIdx));
        return this;
    }

    public ProgrammedPlayerActionsBuilder PlaceCounters(string key, int amount)
    {
        Parent.Result.Actions.Enqueue(new PlaceCountersPPAction(key, amount));
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

    public ProgrammedPlayerActionsBuilder AssertCantActivateItem(string itemKey)
    {
        Parent.Result.Actions.Enqueue(new AssertCantActivateItemPPAction(itemKey));
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

    public ProgrammedPlayerActionsBuilder DeclareAttack(int monsterSlot)
    {
        Parent.Result.Actions.Enqueue(DeclareAttackPPAction.Instance);
        Parent.Result.AttackSlotQueue.Enqueue(monsterSlot);
        return this;
    }

    public ProgrammedPlayerActionsBuilder GainCoins(int amount)
    {
        Parent.Result.Actions.Enqueue(new GainCoinsPPAction(amount));
        return this;
    }

    public ProgrammedPlayerActionsBuilder LootCards(int amount)
    {
        Parent.Result.Actions.Enqueue(new LootCardsPPAction(amount));
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

    public ProgrammedPlayerActionsBuilder DiceRoll(int idx)
    {
        parent.Parent.Result.StackEffectsQueue.Enqueue(
            new DiceRollStackEffectChoice(idx)
        );
        return parent;
    }

    public ProgrammedPlayerActionsBuilder Me()
    {
        parent.Parent.Result.PlayerOrItemChoiceQueue.Add(
            new SelfPlayerChoice()
        );
        return parent;
    }

    public ProgrammedPlayerActionsBuilder Player(int idx)
    {
        parent.Parent.Result.PlayerOrItemChoiceQueue.Add(
            new PlayerChoice(idx)
        );
        return parent;
    }

    public ProgrammedPlayerActionsBuilder Option(int optionIdx)
    {
        parent.Parent.Result.OptionsQueue.Enqueue(optionIdx);
        return parent;
    }

    public ProgrammedPlayerActionsBuilder StartingItem(int ownerIdx = -1)
    {
        parent.Parent.Result.PlayerOrItemChoiceQueue.Add(
            new StartingItemChoice(ownerIdx)
        );
        return parent;
    }

    public ProgrammedPlayerActionsBuilder MonsterInSlot(int slotIdx)
    {
        parent.Parent.Result.PlayerOrItemChoiceQueue.Add(
            new MonsterInSlotChoice(slotIdx)
        );
        return parent;
    }
}

public interface IStackEffectChoice
{
    public string GetEffectId(Match match, int playerIdx);
}

public class DiceRollStackEffectChoice(int idx) : IStackEffectChoice
{
    public string GetEffectId(Match match, int playerIdx)
    {
        var count = idx;
        var rollCount = 0;
        foreach (var effect in match.Stack.Effects)
        {
            if (effect is not RollStackEffect)
            {
                continue;
            }
            ++rollCount;
            if (count > 0)
            {
                --count;
                continue;
            }
            return effect.SID;
        }
        throw new Exception($"Not enough roll stack effects in stack! (want: {idx + 1}, have: {rollCount})");
    }
}

public interface IPlayerOrItemChoice
{
    (TargetType, string) Choose(Match match, int playerIdx);
}

public class PlayerChoice(int choiceIdx) : IPlayerOrItemChoice
{
    public (TargetType, string) Choose(Match match, int playerIdx)
    {
        return (TargetType.PLAYER, choiceIdx.ToString());
    }
}

public class SelfPlayerChoice : IPlayerOrItemChoice
{
    public (TargetType, string) Choose(Match match, int playerIdx)
    {
        return (TargetType.PLAYER, playerIdx.ToString());
    }
}

public class StartingItemChoice(int ownerIdx) : IPlayerOrItemChoice
{
    public (TargetType, string) Choose(Match match, int playerIdx)
    {
        var pIdx = ownerIdx == -1
            ? playerIdx
            : ownerIdx;
        var player = match.GetPlayer(pIdx);
        var startingItems = player.StartingItems();
        if (startingItems.Count != 1)
        {
            throw new Exception($"Invalid starting item count for {nameof(StartingItemChoice)}: {startingItems.Count} (playerIdx: {playerIdx})");
        }
        return (TargetType.ITEM, startingItems[0].IPID);
    }
}

public class MonsterInSlotChoice(int slotIdx) : IPlayerOrItemChoice
{
    public (TargetType, string) Choose(Match match, int playerIdx)
    {
        // TODO check for null
        return (TargetType.ITEM, match.MonsterSlots[slotIdx].Card!.IPID);
    }
}

// public class MonsterInSlotChoice : IPlayerOrItemChoice
// {

// }


public class ProgrammedPlayerController : IPlayerController
{
    public string Character { get; set; } = "";

    public Queue<IProgrammedPlayerAction> Actions { get; } = new();
    public Queue<int> HandCardChoiceQueue { get; } = new();
    public List<IPlayerOrItemChoice> PlayerOrItemChoiceQueue { get; } = new();
    public Queue<int> OptionsQueue { get; } = new();
    public Queue<IStackEffectChoice> StackEffectsQueue { get; } = new();
    public Queue<int> AttackSlotQueue { get; } = new();

    public Queue<IProgrammedPlayerSetup> Setups { get; } = new();

    private (TargetType, string) GetPOMChoice(Match match, int playerIdx, List<TargetType> availableTargets)
    {
        foreach (var choice in PlayerOrItemChoiceQueue)
        {
            var (type, result) = choice.Choose(match, playerIdx);
            if (!availableTargets.Contains(type)) continue;

            return (type, result);
        }

        throw new Exception("TODO write a better exception message");
    }

    public Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint)
    {
        if (OptionsQueue.TryDequeue(out var result))
        {
            return Task.FromResult(options[result]);
        }

        throw new Exception($"Choose string queue is empty (hint: \"{hint}\", options: {string.Join(", ", options)})");
    }

    public Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint)
    {
        var (_, result) = GetPOMChoice(match, playerIdx, [TargetType.PLAYER]);
        return Task.FromResult(int.Parse(result));
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
            (result, next) = await action.Do(match, playerIdx, options);
            if (next) Actions.Dequeue();
        }

        if (!options.Contains(result))
        {
            throw new Exception($"Received action \"{result}\", which is not a valid action! (expected: \"{string.Join(", ", options)}\")");
        }

        return result;
    }

    public async Task Setup(Match match, int playerI)
    {
        while (Setups.TryDequeue(out var setup))
        {
            await setup.Do(match, playerI);
        }
    }

    public Task Update(Match match, int playerI) { return Task.CompletedTask; }

    public Task<string> ChooseStackEffect(Match match, int playerIdx, List<string> options, string hint)
    {
        if (!StackEffectsQueue.TryDequeue(out var choice))
        {
            throw new Exception("Stack effect choice queue is empty");
        }

        var result = choice.GetEffectId(match, playerIdx);
        if (!options.Contains(result))
        {
            throw new Exception($"Stack effect with ID {result} is not a valid option for stack effect choice (options: {string.Join(", ", options)})");
        }
        return Task.FromResult(result);
    }

    public Task<int> ChooseCardInHand(Match match, int playerIdx, List<int> options, string hint)
    {
        if (HandCardChoiceQueue.TryDequeue(out var result))
        {
            // TODO check options
            return Task.FromResult(result);
        }

        throw new Exception("Hand card choice queue is empty");
    }

    public Task<string> ChooseItem(Match match, int playerIdx, List<string> options, string hint)
    {
        var (_, result) = GetPOMChoice(match, playerIdx, [TargetType.ITEM]);
        return Task.FromResult(result);
    }

    public Task<int> ChooseItemToPurchase(Match match, int playerIdx, List<int> options)
    {
        throw new NotImplementedException();
    }

    public Task<int> ChooseMonsterToAttack(Match match, int playerIdx, List<int> options)
    {
        if (AttackSlotQueue.TryDequeue(out var result))
        {
            // TODO check options
            return Task.FromResult(result);
        }

        throw new Exception("Attack slot queue is empty");
    }

    public Task<(TargetType, string)> ChooseMonsterOrPlayer(Match match, int playerIdx, List<string> ipids, List<int> indicies, string hint)
    {
        return Task.FromResult(
            GetPOMChoice(match, playerIdx, [TargetType.ITEM, TargetType.PLAYER])
        );
    }

    public Task<DeckType> ChooseDeck(Match match, int playerIdx, List<DeckType> options, string hint)
    {
        throw new NotImplementedException();
    }

}

public static class ProgrammedPlayerControllers
{
    public static ProgrammedPlayerController AutoPassPlayerController(string characterKey) => new ProgrammedPlayerControllerBuilder(characterKey)
        .ConfigActions()
            .AutoPass()
            .Done()
        .Build();
}
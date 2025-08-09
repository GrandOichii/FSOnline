namespace FSCore.Tests.Setup.Players;

public class ProgrammedPlayerControllerBuilder {
    private readonly ProgrammedPlayerActionsBuilder _actions;

    public ProgrammedPlayerControllerBuilder() {
        _actions = new(this);
    }

    public ProgrammedPlayerControllerBuilder SetCharacter(string key) {
        throw new NotImplementedException();
    }

    public ProgrammedPlayerActionsBuilder ConfigActions() => _actions;

    public ProgrammedPlayerController Build() {
        throw new NotImplementedException();
    }
}

public class ProgrammedPlayerActionsBuilder(ProgrammedPlayerControllerBuilder parent) {
    public ProgrammedPlayerControllerBuilder Done() => parent;

    public ProgrammedPlayerActionsBuilder AssertIsCurrentPlayer() {
        throw new NotImplementedException();
    }

    public ProgrammedPlayerActionsBuilder AssertHasCardsInHand(int amount) {
        throw new NotImplementedException();
    }

    public ProgrammedPlayerActionsBuilder PlayLootCard(int idx) {
        AssertHasCardsInHand(idx + 1);
        throw new NotImplementedException();
    }

    public ProgrammedPlayerActionsBuilder AutoPassUntilEmptyStack() {
        throw new NotImplementedException();
    }

    public ProgrammedPlayerActionsBuilder SetWinner() {
        throw new NotImplementedException();
    }

    public ProgrammedPlayerActionsBuilder AutoPass() {
        throw new NotImplementedException();
    }
}

public class ProgrammedPlayerController : IPlayerController {

    public async Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint)
    {
        throw new NotImplementedException();
    }

    public Task CleanUp(Match match, int playerIdx)
    {
        throw new NotImplementedException();
    }

    public async Task<string> PromptAction(Match match, int playerIdx, IEnumerable<string> options)
    {
        throw new NotImplementedException();
    }

    public async Task Setup(Match match, int playerI)
    {
        throw new NotImplementedException();
    }

    public Task Update(Match match, int playerI) { return Task.CompletedTask; }

    public async Task<string> ChooseStackEffect(Match match, int playerIdx, List<string> options, string hint)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ChooseCardInHand(Match match, int playerIdx, List<int> options, string hint)
    {
        throw new NotImplementedException();
    }

    public async Task<string> ChooseItem(Match match, int playerIdx, List<string> options, string hint)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ChooseItemToPurchase(Match match, int playerIdx, List<int> options)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ChooseMonsterToAttack(Match match, int playerIdx, List<int> options)
    {
        throw new NotImplementedException();
    }

    public async Task<(TargetType, string)> ChooseMonsterOrPlayer(Match match, int playerIdx, List<string> ipids, List<int> indicies, string hint)
    {
        throw new NotImplementedException();
    }

    public async Task<DeckType> ChooseDeck(Match match, int playerIdx, List<DeckType> options, string hint)
    {
        throw new NotImplementedException();
    }

}

public static class ProgrammedPlayerControllers {
    public static ProgrammedPlayerController AutoPassPlayerController(string characterKey) => new ProgrammedPlayerControllerBuilder()
        .SetCharacter(characterKey)
        .ConfigActions()
            .AutoPass()
            .Done()
        .Build();
}
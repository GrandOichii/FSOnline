
namespace FSCore.Matches.Actions;

public class PassAction : IAction
{
    public string ActionWord() => "p";

    public Task Exec(Match match, int playerIdx, string[] args)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetAvailable(Match match, int playerIdx)
    {
        // TODO check if current player can end the turn
        yield return ActionWord();
    }
}

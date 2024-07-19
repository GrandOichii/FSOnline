namespace FSCore.Matches.Players.Controllers;

/// <summary>
/// Player controller, used to control player actions
/// </summary>
public interface IPlayerController {
    /// <summary>
    /// Setup the controller
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    public Task Setup(Match match, int playerIdx);
    /// <summary>
    /// Cleanup the controller
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    public Task CleanUp(Match match, int playerIdx);

    /// <summary>
    /// Update the player about a new match state
    /// </summary>
    /// <param name="match">Match</param>
    /// <param name="playerIdx">Player index</param>
    public Task Update(Match match, int playerIdx);
    /// <summary>
    /// Prompts the player to perform an action
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">Available actions</param>
    /// <returns>Action string</returns>
    public Task<string> PromptAction(Match match, int playerIdx, IEnumerable<string> options);
    /// <summary>
    /// Prompt the user to pick from a list of options
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">List of string options</param>
    /// <param name="hint">Hint</param>
    /// <returns>Picked string</returns>
    public Task<string> ChooseString(Match match, int playerIdx, List<string> options, string hint);
    /// <summary>
    /// Prompt the user to pick a player from a list of options
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">List of player indicies</param>
    /// <param name="hint">Hint</param>
    /// <returns>Chosen player index</returns>
    public Task<int> ChoosePlayer(Match match, int playerIdx, List<int> options, string hint);
}

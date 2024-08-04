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
    /// <summary>
    /// Prompt the user to pick a stack effect SID from a list of options
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">List of stack IDs</param>
    /// <param name="hint">Hint</param>
    /// <returns>Chosen player index</returns>
    public Task<string> ChooseStackEffect(Match match, int playerIdx, List<string> options, string hint);
    /// <summary>
    /// Prompt the user to pick a card in a hand from a list of options
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">List of stack IDs</param>
    /// <param name="hint">Hint</param>
    /// <returns>Chosen player index</returns>
    public Task<int> ChooseCardInHand(Match match, int playerIdx, List<int> options, string hint);
    /// <summary>
    /// Prompt the user to pick an item from a list of options
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">List of item IPIDs</param>
    /// <param name="hint">Hint</param>
    /// <returns>Chosen item IPID</returns>
    public Task<string> ChooseItem(Match match, int playerIdx, List<string> options, string hint);
    /// <summary>
    /// Prompt the user to pick a shop slot or top of treasure deck to purchase
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">List of slot indicies (-1 if top of treasure deck)</param>
    /// <returns>Shop index or -1 if top of treasure deck</returns>
    public Task<int> ChooseItemToPurchase(Match match, int playerIdx, List<int> options); // TODO change options to a tuple - (slot, cost)
    /// <summary>
    /// Prompt the user to pick a monster slot or top of monster deck to attack
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="options">List of slot indicies (-1 if top of monster deck)</param>
    /// <returns>Monster index or -1 if top of treasure deck</returns>
    public Task<int> ChooseMonsterToAttack(Match match, int playerIdx, List<int> options);
    /// <summary>
    /// Propmpt the user to pick a monster or player
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <param name="ipids">List of In-play IDs of pickable monsters</param>
    /// <param name="indicies">List of player indicies</param>
    /// <param name="hint">Hint</param>
    /// <returns>A tuple of target type and identifier</returns>
    public Task<(TargetType, string)> ChooseMonsterOrPlayer(Match match, int playerIdx, List<string> ipids, List<int> indicies, string hint);
}

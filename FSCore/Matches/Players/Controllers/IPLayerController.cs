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
}

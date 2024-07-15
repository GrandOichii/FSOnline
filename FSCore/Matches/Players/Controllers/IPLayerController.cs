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
    public Task Cleanup(Match match, int playerIdx);
}

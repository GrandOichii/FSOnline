namespace FSCore.Matches.Actions;

/// <summary>
/// Action performed by a player
/// </summary>
public interface IAction {
    /// <summary>
    /// Execute the action
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Action executor index</param>
    /// <param name="args">Action arguments</param>
    public Task Exec(Match match, int playerIdx, string[] args);
    
    /// <summary>
    /// Get the name of the action
    /// </summary>
    /// <returns>Name of the action</returns>
    public string ActionWord();

    /// <summary>
    /// Gets all of the available actions of this kind a player can make
    /// </summary>
    /// <param name="match">Parent match</param>
    /// <param name="playerIdx">Player index</param>
    /// <returns>Enumerable of all available actions</returns>
    public IEnumerable<string> GetAvailable(Match match, int playerIdx);
}
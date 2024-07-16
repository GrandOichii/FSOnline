namespace FSCore.Matches.Phases;

/// <summary>
/// Match phase
/// </summary>
public interface IPhase {
    /// <summary>
    /// Get the name of the phase
    /// </summary>
    /// <returns>The name of the phase</returns>
    public string GetName();
    /// <summary>
    /// Execute actions before emitting the name of the phase as a trigger
    /// </summary>
    /// <param name="match">Match</param>
    /// <param name="playerIdx">Current player index</param>
    /// <returns></returns>
    public Task PreEmit(Match match, int playerIdx);
    /// <summary>
    /// Execute actions after emitting the name of the phase as a trigger
    /// </summary>
    /// <param name="match">Match</param>
    /// <param name="playerIdx">Current player idx</param>
    /// <returns></returns>
    public Task PostEmit(Match match, int playerIdx);
}
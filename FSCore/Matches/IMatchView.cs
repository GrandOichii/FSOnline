namespace FSCore.Matches;

/// <summary>
/// Match view
/// </summary>
public interface IMatchView {
    /// <summary>
    /// Initialize the match view
    /// </summary>
    /// <param name="match">The viewed match</param>
    public Task Start(Match match);

    /// <summary>
    /// Updates the view with the new match data
    /// </summary>
    /// <param name="match">The viewed match</param>
    public Task Update(Match match);

    /// <summary>
    /// Ends the view
    /// </summary>
    public Task End();
}
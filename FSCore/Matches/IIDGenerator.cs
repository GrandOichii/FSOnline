namespace FSCore.Matches;

/// <summary>
/// ID generator
/// </summary>
public interface IIDGenerator {
    /// <summary>
    /// Generates new ID
    /// </summary>
    /// <returns>New ID</returns>
    public string Next();
}
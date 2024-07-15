namespace FSCore.Matches.Players;

/// <summary>
/// Participant of a match
/// </summary>
public class Player {
    /// <summary>
    /// Parent match
    /// </summary>
    public Match Match { get; }
    /// <summary>
    /// Player name
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Platyer index
    /// </summary>
    public int Idx { get; }
    /// <summary>
    /// Player controller
    /// </summary>
    public IPlayerController Controller { get; }

    public Player(Match match, string name, int idx, IPlayerController controller) {
        Match = match;
        Name = name;
        Idx = idx;
        Controller = controller;

    }
}
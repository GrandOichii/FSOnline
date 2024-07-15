using Microsoft.Extensions.Logging;

namespace FSCore.Matches;

/// <summary>
/// Match process
/// </summary>
public class Match {
    /// <summary>
    /// Random number generator
    /// </summary>
    public Random Rng { get; }
    /// <summary>
    /// Match configuration
    /// </summary>
    public MatchConfig Config { get; }
    /// <summary>
    /// Card master
    /// </summary>
    private readonly ICardMaster _cardMaster;
    /// <summary>
    /// Index of the current player
    /// </summary>
    public int CurPlayerIdx { get; set; }
    /// <summary>
    /// Lua state object
    /// </summary>
    public Lua LState { get; } = new();
    /// <summary>
    /// Match info internal logger
    /// </summary>
    public ILogger? Logger { get; set; } = null;
    public List<Player> Players { get; }

    public Match(MatchConfig config, int seed, ICardMaster cardMaster, string setupScript) {
        _cardMaster = cardMaster;
        Config = config;

        Rng = new(seed);
        CurPlayerIdx = 0;
        if (config.RandomFirstPlayer)
            CurPlayerIdx = Rng.Next() % 2;
        Players = new();

        LogInfo("Running setup script");
        LState.DoString(setupScript);

        _ = new ScriptMaster(this);
    }

    #region Logging

    /// <summary>
    /// Log info using system logger
    /// </summary>
    /// <param name="info">Info</param>
    public void LogInfo(string info) {
        Logger?.LogInformation(info);
    }

    /// <summary>
    /// Log warning using system logger
    /// </summary>
    /// <param name="msg">Warning message</param>
    public void LogWarning(string msg) {
        Logger?.LogWarning(msg);
    }

    /// <summary>
    /// Log err using system logger and crash the match
    /// </summary>
    /// <param name="msg">Error message</param>
    /// <exception cref="MatchException"></exception>
    public void LogError(string msg) {
        Logger?.LogError(msg);
        
        throw new MatchException(msg);
    }

    #endregion


    public async Task AddPlayer(string name, IPlayerController controller) {
        // TODO character
        var player = new Player(
            this,
            name,
            Players.Count,
            controller
        );

        Players.Add(player);
    }
}
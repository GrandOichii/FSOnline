namespace FSCore.Matches;

public class Match {
    public Random Rng { get; }
    public MatchConfig Config { get; }
    private readonly ICardMaster _cardMaster;
    
     public Match(MatchConfig config, int seed, ICardMaster cardMaster, string setupScript) {
        _cardMaster = cardMaster;
        Config = config;

        Rng = new(seed);
        // CurPlayerI = 0;
        // if (config.RandomFirstPlayer)
        //     CurPlayerI = Rng.Next() % 2;

        // LogInfo("Running setup script");
        // LState.DoString(setupScript);

        // _ = new ScriptMaster(this);
        // LastState = new();

        // UEOTEffects = new();
        // AEOTEffects = new();
    }
}
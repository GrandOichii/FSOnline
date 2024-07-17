using Microsoft.Extensions.Logging;

namespace FSCore.Matches;

/// <summary>
/// Match process
/// </summary>
public class Match {
    /// <summary>
    /// Turn structure
    /// </summary>
    private static readonly List<IPhase> _phases = new(){
        new StartPhase(),
        new ActionPhase(),
        new EndPhase(),
    };

    /// <summary>
    /// Card ID generator
    /// </summary>
    public IIDGenerator CardIDGenerator { get; set; } = new BasicIDGenerator();
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
    /// <summary>
    /// Match view
    /// </summary>
    public IMatchView? View { get; set; } = null;
    /// <summary>
    /// Match participants
    /// </summary>
    public List<Player> Players { get; }
    /// <summary>
    /// Player index of the winner
    /// </summary>
    public int WinnerIdx { get; private set; }
    /// <summary>
    /// Current phase
    /// </summary>
    public IPhase CurrentPhase { get; private set; }
    /// <summary>
    /// Coin pool
    /// </summary>
    public int CoinPool { get; private set; }
    /// <summary>
    /// Indicates whether the current player's turn has ended
    /// </summary>
    public bool TurnEnded { get; set; }
    /// <summary>
    /// Effect stack
    /// </summary>
    public Stack Stack { get; }

    #region Decks

    public Deck LootDeck { get; }

    public Dictionary<DeckType, Deck> DeckIndex { get; }

    #endregion

    /// <summary>
    /// Shows whether the match is active (no winner is yet decided)
    /// </summary>
    public bool Active => WinnerIdx < 0;
    /// <summary>
    /// Current player
    /// </summary>
    public Player CurrentPlayer => Players[CurPlayerIdx];

    public Match(MatchConfig config, int seed, ICardMaster cardMaster, string setupScript) {
        _cardMaster = cardMaster;
        Config = config;
        CoinPool = config.CoinPool;

        CurrentPhase = new MatchSetupPhase();
        Rng = new(seed);
        CurPlayerIdx = 0;
        if (config.RandomFirstPlayer)
            CurPlayerIdx = Rng.Next() % 2;
        Players = new();
        WinnerIdx = -1;

        Stack = new(this);
        LootDeck = new(this, true);
        DeckIndex = new() {
            { DeckType.LOOT, LootDeck },
        };
        // TODO populate loot deck in setup

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

    /// <summary>
    /// Add a player to the match
    /// </summary>
    /// <param name="name"></param>
    /// <param name="controller"></param>
    /// <returns></returns>
    public async Task AddPlayer(string name, IPlayerController controller, string characterKey = "") {
        if (Players.Count == Config.MaxPlayerCount)
            throw new MatchException($"tried to add another player to the match, while it is already full (max player count: {Config.MaxPlayerCount})");

        var character = string.IsNullOrEmpty(characterKey)
            ? await _cardMaster.GetRandomCharacter(Rng)
            : await _cardMaster.GetCharacter(characterKey);

        var player = new Player(
            this,
            name,
            Players.Count,
            character,
            controller
        );

        Players.Add(player);
    }

    /// <summary>
    /// Starts the match execution
    /// </summary>
    /// <exception cref="MatchException"></exception>
    public async Task Run() {
        await SetupDecks();
        await SetupView();
        await SetupPlayers();
        await Turns();
        await CleanUp();
    }

    /// <summary>
    /// Creates a list of loot cards using the card keys provided in the configuration object
    /// </summary>
    /// <returns>List of loot cards</returns>
    private async Task<List<MatchCard>> CreateLootCards() {
        var result = new List<MatchCard>();
        foreach (var pair in Config.LootCards) {
            var amount = pair.Value;
            var card = await _cardMaster.Get(pair.Key);
            for (int i = 0; i < amount; i++) {
                result.Add(new(this, card));
            }
        }
        return result;
    }

    /// <summary>
    /// Sets up the decks
    /// </summary>
    public async Task SetupDecks() {
        // loot deck
        var lootCards = await CreateLootCards();
        LootDeck.Populate(lootCards);

        // treasure deck
        // TODO

        // monster deck
        // TODO

        // monster deck
        // TODO

        // shuffle
        foreach (var deck in DeckIndex.Values)
            deck.Shuffle();
    }

    /// <summary>
    /// Sets up the match
    /// </summary>
    private async Task SetupView() {
        LogInfo("Starting view");
        View?.Start(this);

        LogInfo("Setup complete");
    }

    /// <summary>
    /// Sets up the players, participating in the match
    /// </summary>
    private async Task SetupPlayers() {
        foreach (var player in Players) {
            LogInfo($"Running setup for player {player.LogName}");
            await player.Setup();
        }
        
        // TODO add back
        // LogInfo("Pushing initial state");
        // await PushUpdates();
    }

    /// <summary>
    /// Cleans up after the match is done executing
    /// </summary>
    private async Task CleanUp() {
        LogInfo("Performing cleanup");

        foreach (var player in Players) {
            await player.CleanUp();
        }

        if (View is not null)
            await View.End();
    }

    /// <summary>
    /// Goes over all of the players' turns until a winner is decided
    /// </summary>
    private async Task Turns() {
        LogInfo("Started main match loop");

        // TODO add Logger back
        // Logger.Log("Match started");
        while (Active) {
            // TurnCount++;

            await ReloadState();
            if (!Active) return;

            var cPlayer = CurrentPlayer;

            LogInfo($"Player {cPlayer.LogName} starts their turn");
            // Logger.Log(cPlayer.Name + " started their turn.");

            foreach (var phase in _phases) {
                CurrentPhase = phase;
                var phaseName = CurrentPhase.GetName();
                await phase.PreEmit(this, CurPlayerIdx);
                await Emit(phaseName, new(){ {"playerI", CurPlayerIdx} });
                await phase.PostEmit(this, CurPlayerIdx);
                
                await ReloadState();
                if (!Active) return;
            }
            // Logger.Log("Player " + cPlayer.Name + " passed their turn.");
        }
    }

    /// <summary>
    /// Decides the index of the next player
    /// </summary>
    public void AdvanceCurrentPlayerIdx() {
        // TODO more complex
        CurPlayerIdx = NextInTurnOrder(CurPlayerIdx);
    }

    /// <summary>
    /// Reloads the state and pushes the updates
    /// </summary>
    public async Task ReloadState() {
        // TODO
    }

    /// <summary>
    /// Emits a trigger
    /// </summary>
    /// <param name="trigger">Trigger name</param>
    /// <param name="args">Trigger arguments</param>
    public async Task Emit(string trigger, Dictionary<string, object> args) {
        var logMessage = $"Emitted trigger {trigger}, args: ";
        var argsTable = LuaUtility.CreateTable(LState, args);
        foreach (var pair in args) {
            logMessage += $"{pair.Key}:{pair.Value} ";
        }
        LogInfo(logMessage);
        await ReloadState();

        // TODO check cards

        // foreach (var player in LastState.Players) {
        //     // heroes
        //     var hero = player.Original.Hero;
        //     if (hero is not null && hero.TriggeredAbilities.Count > 0) {
        //         foreach (var trigger in hero.TriggeredAbilities) {
        //             var on = trigger.Trigger;
        //             if (on != trigger) continue;

        //             var canTrigger = trigger.ExecCheck(player, argsTable);
        //             if (!canTrigger) {
        //                 continue;
        //             }

        //             var payedCosts = trigger.ExecCosts(player, argsTable);
        //             if (!payedCosts) {
        //                 continue;
        //             }

        //             LogInfo($"Hero card {hero.LogFriendlyName} triggers!");
        //             trigger.ExecEffect(player, argsTable);
        //         }
        //     }

        //     // landscapes
        //     foreach (var lane in player.Landscapes) {
        //         var cards = new List<InPlayCardState>();
        //         if (lane.Creature is not null && lane.Creature.TriggeredAbilities.Count > 0) {
        //             cards.Add(lane.Creature);
        //         }
        //         cards.AddRange(lane.Buildings);
        //         foreach (var card in cards) {
        //             foreach (var trigger in card.TriggeredAbilities) {
        //                 var on = trigger.Trigger;
        //                 if (on != trigger) continue;

        //                 var canTrigger = trigger.ExecCheck(player, card, lane.Original.Idx, argsTable);
        //                 if (!canTrigger) {
        //                     continue;
        //                 }

        //                 var payedCosts = trigger.ExecCosts(player, card, lane.Original.Idx, argsTable);
        //                 if (!payedCosts) {
        //                     continue;
        //                 }

        //                 LogInfo($"Card {card.Original.Card.LogFriendlyName} triggers!");
        //                 trigger.ExecEffect(player, card, lane.Original.Idx, argsTable);
        //             }
        //         }
        //     }
        // }

        LogInfo($"Finished emitting {trigger}");
    }

    /// <summary>
    /// Remove coins from the pool
    /// </summary>
    /// <param name="amount">Number of coins to be removed</param>
    /// <returns>Actual amount of coins removed</returns>
    public int TakeCoins(int amount) {
        if (CoinPool < 0) return amount;

        CoinPool -= amount;
        if (CoinPool < 0) {
            amount -= -CoinPool;
            CoinPool = 0;
        }

        return amount;
    }

    public void AddToCoinPool(int amount) {
        CoinPool += amount;
        if (CoinPool > Config.CoinPool)
            throw new MatchException($"Unexpected scenario: coin pool is larger than provided in configuration (expected: {Config.CoinPool}, actual: {CoinPool})");
    }

    #region Deck manipulation

    /// <summary>
    /// Remove the top N cards from the loot deck
    /// </summary>
    /// <param name="amount">Amount of cards to be removed</param>
    /// <returns>Removed cards</returns>
    public List<MatchCard> RemoveCardsFromTopOfLootDeck(int amount) {
        var result = LootDeck.RemoveTop(amount);

        LogInfo($"Removed {result.Count} cards from the loot deck");

        return result;
    }

    /// <summary>
    /// Places the card into the loot discard
    /// </summary>
    /// <param name="card">Match card</param>
    public void PlaceIntoLootDiscard(MatchCard card) {
        LootDeck.PlaceIntoDiscard(card);

        LogInfo($"Card {card.LogName} is put into discard of loot deck");
    }

    #endregion

    /// <summary>
    /// Gets a player by their index
    /// </summary>
    /// <param name="idx">Player index</param>
    /// <returns>Player associated with the index</returns>
    public Player GetPlayer(int idx) => Players[idx];

    #region Stack

    /// <summary>
    /// Get the player with priority/current player
    /// </summary>
    /// <returns>Player with priority/current player</returns>
    public Player GetPriorityPlayer() {
        var pIdx = Stack.PriorityIdx;
        if (pIdx != -1) {
            return Players[pIdx];
        }
        return CurrentPlayer;
    }

    /// <summary>
    /// TODO add docs
    /// </summary>
    /// <returns></returns>
    public async Task ResolveStack() {
        await Stack.Resolve();
    }

    /// <summary>
    /// Process a pass action from a player
    /// </summary>
    /// <param name="player">Action owner</param>
    public async Task ProcessPass(Player player) {
        var shouldEnd = await Stack.ProcessPass(player);
        if (!shouldEnd) return;

        // check if turn should end
        if (player.Idx == CurPlayerIdx) {
            TurnEnded = true;
            return;
        }

        throw new MatchException($"Unknown scenario: player {player.LogName} tried to pass, but didn't have a reason to");
    }

    /// <summary>
    /// Place loot card on the stack
    /// </summary>
    /// <param name="ownerIdx">Effect owner</param>
    /// <param name="card">Loot card</param>
    public async Task PlaceOnStack(int ownerIdx, HandMatchCard card) {
        var effect = new LootCardStackEffect(this, ownerIdx, card.Card);
        Stack.AddEffect(effect);
    }

    /// <summary>
    /// Place card activation on stack
    /// </summary>
    /// <param name="ability">Activated ability</param>
    /// <param name="card">Activated card</param>
    /// <param name="owner"></param>
    /// <returns></returns>
    public async Task PlaceOnStack(ActivatedAbility ability, InPlayMatchCard card, Player owner) {
        var effect = new ActivatedAbilityStackEffect(ability, card, owner);
        Stack.AddEffect(effect);
    }

    #endregion

    /// <summary>
    /// Throw exception if strict mode is enabled, else log a warning to system logger
    /// </summary>
    /// <param name="errMsg">Error message</param>
    public void PotentialError(string errMsg) {
        if (Config.StrictMode)
            LogError(errMsg);

        LogWarning(errMsg);
    }

    #region Card IDs

    /// <summary>
    /// Generate a new ID for a match card
    /// </summary>
    /// <returns>New ID</returns>
    public string GenerateCardID() {
        var result = CardIDGenerator.Next();
        LogInfo($"Generated match card ID: {result}");
        return result;
    }

    public string GenerateInPlayID() {
        var result = "i" + CardIDGenerator.Next();
        LogInfo($"Generated item match card ID: {result}");
        return result;
    }

    #endregion

    public int NextInTurnOrder(int playerIdx) {
        // TODO change if there are cards that change the turn order
        return (playerIdx + 1) % Players.Count;
    }

    #region In-play cards

    public InPlayMatchCard? GetInPlayCardOrDefault(string ipid) {
        foreach (var player in Players) {
            var result = player.GetInPlayCardOrDefault(ipid);
            if (result is not null) return result;
        }

        return null;
    }

    public InPlayMatchCard GetInPlayCard(string ipid) {
        return GetInPlayCardOrDefault(ipid)
            ?? throw new MatchException($"Failed to get in-play card with IPID {ipid}")
        ;
    }

    public async Task<List<OwnedInPlayMatchCard>> CreateStartingItems(Player owner, CharacterCardTemplate characterTemplate) {
        var result = new List<OwnedInPlayMatchCard>();

        foreach (var key in characterTemplate.StartingItems) {
            var template = await _cardMaster.Get(key);
            var card = new OwnedInPlayMatchCard(
                new MatchCard(this, template),
                owner
            );
            result.Add(card);
        }

        return result;
    }

    #endregion
}
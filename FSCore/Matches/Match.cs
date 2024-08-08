using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace FSCore.Matches;

/// <summary>
/// Match process
/// </summary>
public class Match {
    /// <summary>
    /// Turn structure
    /// </summary>
    private static readonly List<IPhase> _phases = [
        new StartPhase(),
        new ActionPhase(),
        new EndPhase(),
    ];
    private static readonly List<ModificationLayer> MODIFICATION_LAYERS = new() {
        ModificationLayer.COIN_GAIN_AMOUNT,
        ModificationLayer.LOOT_AMOUNT,
        ModificationLayer.ROLL_REPLACEMENT_EFFECTS,
        ModificationLayer.HAND_CARD_VISIBILITY,
        ModificationLayer.LOOT_PLAY_RESTRICTIONS,
        ModificationLayer.ITEM_ACTIVATION_RESTRICTIONS,
        ModificationLayer.PURCHASE_COST,
        ModificationLayer.ITEM_DESTRUCTION_REPLACEMENT_EFFECTS,
        ModificationLayer.MOD_MAX_LOOT_PLAYS,

        // player stats
        ModificationLayer.PLAYER_MAX_HEALTH,
        ModificationLayer.PLAYER_ATTACK,

        // montser stats
        ModificationLayer.MONSTER_HEALTH,
        ModificationLayer.MONSTER_EVASION,
        ModificationLayer.MONSTER_ATTACK,

        // death penalty
        ModificationLayer.DEATH_PENALTY_MODIFIERS,
        ModificationLayer.DEATH_PENALTY_REPLACEMENT_EFFECTS,

        ModificationLayer.LAST,
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
    /// <summary>
    /// Bonus souls
    /// </summary>
    public List<BonusSoulMatchCard> BonusSouls { get; }
    /// <summary>
    /// "Till end of turn" effects
    /// </summary>
    public Dictionary<ModificationLayer, List<LuaFunction>> TEOTEffects { get; }
    /// <summary>
    /// Cards that died this turn
    /// </summary>
    public List<MatchCard> DeadCards { get; } = [];

    #region Decks

    /// <summary>
    /// Loot deck and discard
    /// </summary>
    public Deck LootDeck { get; }
    /// <summary>
    /// Treasure deck and discard
    /// </summary>
    public Deck TreasureDeck { get; }
    /// <summary>
    /// Room deck and discard
    /// </summary>
    public Deck RoomDeck { get; }
    /// <summary>
    /// Monster deck and discard
    /// </summary>
    public Deck MonsterDeck { get; }

    /// <summary>
    /// Index of all decks
    /// </summary>
    public Dictionary<DeckType, Deck> DeckIndex { get; }

    #endregion

    #region Slots

    public List<TreasureSlot> TreasureSlots { get; }
    public List<RoomSlot> RoomSlots { get; }
    public List<MonsterSlot> MonsterSlots { get; }

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
        Players = [];
        WinnerIdx = -1;

        Stack = new(this);
        LootDeck = new(this, true);
        TreasureDeck = new(this, true);
        RoomDeck = new(this, true);
        MonsterDeck = new(this, true);

        TreasureSlots = [];
        RoomSlots = [];
        MonsterSlots = [];
        BonusSouls = [];
        DeckIndex = new() {
            { DeckType.LOOT, LootDeck },
            { DeckType.TREASURE, TreasureDeck },
            { DeckType.ROOM, RoomDeck },
            { DeckType.MONSTER, MonsterDeck },
        };
        TEOTEffects = [];

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
            ? await _cardMaster.GetRandomCharacter(Rng, Config.Characters)
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
        await SetupView();
        await SetupDecks();
        await SetupSlots();
        await SetupPlayers();
        await Turns();
        await CleanUp();
    }

    /// <summary>
    /// Setup the slots
    /// </summary>
    public async Task SetupSlots() {
        // treasure
        LogInfo($"Filling treasure slots (initial count: {Config.InitialTreasureSlots})");
        for (int i = 0; i < Config.InitialTreasureSlots; i++) {
            var slot = new TreasureSlot(TreasureDeck, i);
            await slot.Fill();
            TreasureSlots.Add(slot);
        }

        // monsters
        LogInfo($"Filling monster slots (initial count: {Config.InitialMonsterSlots})");
        for (int i = 0; i < Config.InitialMonsterSlots; i++) {
            var slot = new MonsterSlot(MonsterDeck, i);
            await slot.Fill();
            MonsterSlots.Add(slot);
        }
        // add events
        foreach (var key in Config.Events)
            MonsterDeck.Cards.AddLast(new MatchCard(
                this,
                await _cardMaster.Get(key),
                DeckType.MONSTER
            ));
        // add curses
        foreach (var key in Config.Curses)
            MonsterDeck.Cards.AddLast(new MatchCard(
                this,
                await _cardMaster.Get(key),
                DeckType.MONSTER
            ));
        MonsterDeck.Shuffle();

        // rooms
        if (Config.UseRooms) {
            LogInfo($"Filling room slots (initial count: {Config.InitialRoomSlots})");
            for (int i = 0; i < Config.InitialRoomSlots; i++) {
                var slot = new RoomSlot(RoomDeck, i);
                await slot.Fill();
                RoomSlots.Add(slot);
            }
        }
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
                result.Add(new(this, card, DeckType.LOOT));
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
        var treasureCards = new List<MatchCard>();
        foreach (var key in Config.Treasures)
            treasureCards.Add(new MatchCard(
                this,
                await _cardMaster.Get(key),
                DeckType.TREASURE
            ));

        TreasureDeck.Populate(treasureCards);

        // bonus souls
        var souls = new List<BonusSoulMatchCard>();
        var keys = Config.BonusSouls.OrderBy(k => Rng.Next()).Take(5);
        foreach (var key in keys)
            souls.Add(new BonusSoulMatchCard(
                this,
                await _cardMaster.Get(key)
            ));
        BonusSouls.AddRange(souls);

        // monster deck
        var monsters = new List<MatchCard>();
        foreach (var key in Config.Monsters)
            monsters.Add(new MatchCard(
                this,
                await _cardMaster.GetMonster(key),
                DeckType.MONSTER
            ));
        MonsterDeck.Populate(monsters);
        // TODO shuffle all of the monster cards into the deck, create the monster slots, THEN add all of the event cards and shuffle the monster deck again

        // rooms
        if (Config.UseRooms) {
            var roomCards = new List<MatchCard>();
            foreach (var key in Config.Rooms)
                roomCards.Add(new MatchCard(
                    this,
                    await _cardMaster.Get(key),
                    DeckType.ROOM
                ));

            RoomDeck.Populate(roomCards);
        }

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
        
        LogInfo("Pushing initial state");
        await PushUpdates();
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
                await Emit(phaseName, new(){ {"playerIdx", CurPlayerIdx} });
                await DequeueTriggers();
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
        await SoftReloadState();
        await CheckWinners();
        if (!Active) return;

        await DequeueTriggers();

        await PushUpdates();
    }

    /// <summary>
    /// Checks whether any of the players are considered as winners
    /// </summary>
    /// <returns></returns>
    public async Task CheckWinners() {
        // TODO? multiple winners

        foreach (var player in Players) {
            if (player.Wins()) {
                WinnerIdx = player.Idx;
                return;
            }
        }
    }

    public async Task PushUpdates() {
        if (View is not null) {
            await View.Update(this);
        }
        
        foreach (var player in Players) {
            await player.UpdateController();
        }
    }

    public async Task SoftReloadState() {
        // players
        foreach (var player in Players)
            player.UpdateState();

        // monsters
        foreach (var slot in MonsterSlots) {
            if (slot.Card is null) continue;

            slot.Card.UpdateState();
        }

        // rooms
        foreach (var slot in RoomSlots) {
            if (slot.Card is null) continue;

            slot.Card.UpdateState();
        }

        // monsters
        foreach (var slot in MonsterSlots) {
            if (slot.Card is null) continue;

            slot.Card.UpdateState();
        }

        foreach (var layer in MODIFICATION_LAYERS) {
            // players
            foreach (var player in Players)
                player.Modify(layer);

            // bonus souls
            var souls = new List<BonusSoulMatchCard>(BonusSouls);
            foreach (var soul in souls)
                soul.Modify(layer);

            // "till end of turn" effects
            if (TEOTEffects.TryGetValue(layer, out List<LuaFunction>? mods)) {
                try {
                    foreach (var mod in mods) {
                        mod.Call();
                    }
                } catch (Exception e) {
                    throw new MatchException($"Failed to execute \"till end of turn\" effect in layer {layer}", e);
                }
            }

            // rooms
            foreach (var slot in RoomSlots) {
                if (slot.Card is null) continue;

                slot.Card.Modify(layer);
            }

            // monsters
            foreach (var slot in MonsterSlots) {
                if (slot.Card is null) continue;

                slot.Card.Modify(layer);
            }
        }

        // check player deaths
        foreach (var player in Players)
            await player.CheckDead();
        
        foreach (var item in GetInPlayCards())
            await item.CheckDead();
    }

    #region Triggers

    public async Task DequeueTriggers() {
        while (Stack.QueuedTriggers.Count > 0) {
            var trigger = Stack.QueuedTriggers.Dequeue();
            await ProcessTrigger(trigger);
        }
    }

    private async Task ProcessTrigger(QueuedTrigger trigger) {
        // TODO monsters

        // owned items
        foreach (var player in Players) {
            var items = player.GetInPlayCards();
            // TODO prompt the player to order the effects
            foreach (var item in items) {
                await item.ProcessTrigger(trigger);
            }
        }

        // treasure slots
        foreach (var slot in TreasureSlots) {
            await slot.ProcessTrigger(trigger);
        }

        // rooms
        foreach (var slot in RoomSlots) {
            await slot.ProcessTrigger(trigger);
        }

        // monster slots
        foreach (var slot in MonsterSlots) {
            await slot.ProcessTrigger(trigger);
        }
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
        var queued = new QueuedTrigger(trigger, argsTable);
        Stack.QueueTrigger(queued);
    }

    #endregion

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
    /// Place the card into it's origin deck
    /// </summary>
    /// <param name="card">Match card</param>
    /// <returns></returns>
    public async Task PlaceIntoDiscard(MatchCard card) {
        if (card.DeckOrigin is null) {
            return;
            throw new MatchException($"Tried to put card {card.LogName} into discard, while it has no deck origin");
        }

        DeckType origin = (DeckType)card.DeckOrigin;
        var deck = DeckIndex[origin];
        deck.PlaceIntoDiscard(card);

        LogInfo($"Card {card.LogName} was put into discard of deck {card.DeckOrigin}");
    }

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
    /// Remove the top N cards from the treasure deck
    /// </summary>
    /// <param name="amount">Amount of cards to be removed</param>
    /// <returns>Removed cards</returns>
    public List<MatchCard> RemoveCardsFromTopOfTreasureDeck(int amount) {
        var result = TreasureDeck.RemoveTop(amount);

        LogInfo($"Removed {result.Count} cards from the treasure deck");

        return result;
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
    /// Cancel the effect with the specified Stack ID
    /// </summary>
    /// <param name="sid">Stack ID</param>
    public void CancelEffect(string sid) {
        var effect = Stack.Effects.First(e => e.SID == sid);
        effect.Cancelled = true;
        Stack.Effects.Remove(effect);
        // TODO? more?
    }

    /// <summary>
    /// Get the player with priority/current player
    /// </summary>
    /// <returns>Player with priority/current player</returns>
    public Player GetPriorityPlayer() {
        var pIdx = Stack.PriorityIdx;
        if (pIdx >= 0) {
            return Players[pIdx];
        }
        return CurrentPlayer;
    }

    /// <summary>
    /// Resolve the stack
    /// </summary>
    /// <returns></returns>
    public async Task ResolveStack(bool breakIfPass = false) {
        await Stack.Resolve(breakIfPass);
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
        System.Console.WriteLine(Stack.Effects.Count);
        System.Console.WriteLine(player.LogName + " " + player.Idx + " " + CurPlayerIdx);
        throw new MatchException($"Unknown scenario: player {player.LogName} tried to pass, but didn't have a reason to");
    }

    /// <summary>
    /// Remove the top effect of the stack
    /// </summary>
    public void RemoveTopOfStack() {
        Stack.Effects.Remove(Stack.Top);
    }

    /// <summary>
    /// Palce an effect on top of the stack
    /// </summary>
    /// <param name="effect">Stack effect</param>
    public async Task PlaceOnStack(StackEffect effect) {
        Stack.AddEffect(effect);
    }

    /// <summary>
    /// Roll a dice and place the result onto the stack
    /// </summary>
    /// <param name="parent">Parent effect</param>
    /// <returns></returns>
    public async Task AddRoll(StackEffect parent) {
        var effect = new RollStackEffect(this, parent);
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
        // LogInfo($"Generated match card ID: {result}");
        return result;
    }

    /// <summary>
    /// Generate a new ID for a stack effect
    /// </summary>
    /// <returns>New ID</returns>
    public string GenerateStackID() {
        var result = "s" + CardIDGenerator.Next();
        // LogInfo($"Generated stack effect ID: {result}");
        return result;
    }

    /// <summary>
    /// Generate a new ID for an in-play match card
    /// </summary>
    /// <returns>New ID</returns>
    public string GenerateInPlayID() {
        var result = "i" + CardIDGenerator.Next();
        // LogInfo($"Generated item match card ID: {result}");
        return result;
    }

    #endregion

    public int NextInTurnOrder(int playerIdx) {
        // TODO change if there are cards that change the turn order
        return (playerIdx + 1) % Players.Count;
    }

    #region In-play cards

    public async Task OnCardEnteredPlay(InPlayMatchCard card) {
        await Emit("item_enter", new() {
            { "Card", card },
        });
    }

    public async Task DiscardFromPlay(string ipid) {
        await DiscardFromPlay(GetInPlayCard(ipid));
    }

    public async Task<bool> TryDiscardFromPlay(string ipid) {
        var card = GetInPlayCardOrDefault(ipid);
        if (card is null) return false;

        await DiscardFromPlay(ipid);
        return true;
    }

    public async Task RemoveFromPlay(InPlayMatchCard card) {
        // players
        // TODO trigger and soft reload state, then discard the card

        if (card is OwnedInPlayMatchCard ownedCard) {
            LogInfo($"Removing card {ownedCard.LogName} from player {ownedCard.Owner.LogName}");
            await ownedCard.Owner.RemoveFromPlay(ownedCard);
        }

        // shop items
        foreach (var slot in TreasureSlots) {
            if (slot.Card == card) {
                LogInfo($"Removing card {card.LogName} from {slot.Name} slot [{slot.Idx}]");
                await slot.Fill();
                break;
            }
        }

        // room slots
        foreach (var slot in RoomSlots) {
            if (slot.Card == card) {
                LogInfo($"Removing card {card.LogName} from {slot.Name} slot [{slot.Idx}]");
                await slot.Fill();
                break;
            }
        }

        // monster slots
        foreach (var slot in MonsterSlots) {
            if (slot.Card == card) {
                LogInfo($"Removing card {card.LogName} from {slot.Name} slot [{slot.Idx}]");
                await slot.Fill();
                break;
            }
        }

    }

    public async Task DiscardFromPlay(InPlayMatchCard card) {
        await RemoveFromPlay(card);
        await PlaceIntoDiscard(card.Card);
    }

    public async Task<bool> StealItem(int playerIdx, string ipid) {
        var item = GetInPlayCardOrDefault(ipid);
        if (item is null) return false;
        var newOwner = GetPlayer(playerIdx);

        if (item is OwnedInPlayMatchCard ownedItem) {
            await ownedItem.Owner.LoseItem(ownedItem);
            await newOwner.GainItem(ownedItem);
            ownedItem.SetOwner(newOwner);
            return true;
        }

        foreach (var slot in TreasureSlots) {
            var card = slot.Card;
            if (card is null || card.IPID != ipid) continue;
            await newOwner.GainItem(new(card, newOwner));
            await slot.Fill();
            return true;
        }

        throw new MatchException($"Failed to find source of item {item.LogName} to steal to player {newOwner.LogName}");
    }

    public async Task RerollItem(string ipid) {
        var item = GetInPlayCard(ipid);

        await DestroyItem(ipid);

        if (item is OwnedInPlayMatchCard ownedItem) {
            var owner = ownedItem.Owner;
            var result = await owner.GainTreasureRaw(1);
            if (result.Count == 0) {
                LogInfo($"Item {item.LogName} of player {owner.LogName} was rerolled with no replacement");
                return;
            }
            LogInfo($"Item {item.LogName} of player {owner.LogName} was rerolled into {result[0].LogName}");
        }
    }

    public List<InPlayMatchCard> GetItems() {
        var result = new List<InPlayMatchCard>();

        foreach (var player in Players)
            result.AddRange(player.Items);

        foreach (var slot in TreasureSlots)
            if (slot.Card is not null)
                result.Add(slot.Card);

        return result;
    }

    public InPlayMatchCard? GetItemOrDefault(string ipid) {
        return GetItems().FirstOrDefault(item => item.IPID == ipid);
    }

    public InPlayMatchCard? GetInPlayCardOrDefault(string ipid) {
        return GetInPlayCards().FirstOrDefault(card => card.IPID == ipid);
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

    public async Task PlaceOwnedCard(OwnedInPlayMatchCard card, bool triggerEnter = true) {
        LogInfo($"Item {card.LogName} enters play under control of {card.Owner.LogName}");

        if (triggerEnter)
            await OnCardEnteredPlay(card);

        await card.Owner.GainItem(card);
    }

    public async Task<bool> DestroyItem(string ipid) {
        var card = GetInPlayCard(ipid);
        return await DestroyItem(card);
    }

    public async Task<bool> DestroyItem(InPlayMatchCard card) {
        try {
            foreach (var effect in card.State.DestructionReplacementEffects) {
                var returned = effect.Call(card);
                if (LuaUtility.GetReturnAsBool(returned)) return false;
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to call destruction replacement effect of card {card.LogName}", e);
        }

        await DiscardFromPlay(card);
        return true;
    }

    #endregion

    #region Souls

    public async Task AddSoulCard(int playerIdx, MatchCard card) {
        var player = GetPlayer(playerIdx);
        var soul = new SoulCard(card);

        await player.AddSoulCard(soul);
    }

    #endregion

    #region Shop

    public async Task ExpandShotSlots(int amount) {
        LogInfo($"Expanding shop slots by {amount}");
        for (int i = 0; i < amount; i++) {
            var slot = new TreasureSlot(TreasureDeck, TreasureSlots.Count);
            await slot.Fill();
            TreasureSlots.Add(slot);           
        }
    }

    #endregion

    public async Task DamageToPlayerRequest(int toIdx, int amount, StackEffect source) {
        var effect = new DamageStackEffect(this, -1, amount, source, GetPlayer(toIdx));
        await PlaceOnStack(effect);
    }

    public async Task DamageToCardRequest(string ipid, int amount, StackEffect source) {
        var effect = new DamageStackEffect(this, source.OwnerIdx, amount, source, GetInPlayCard(ipid));
        await PlaceOnStack(effect);
    }

    public InPlayMatchCard GetMonster(string ipid) {
        return MonsterSlots
            .Where(slot => slot.Card is not null)
            .Select(slot => slot.Card!)
            .First(card => card.IPID == ipid)
        ;
    }

    public List<InPlayMatchCard> GetInPlayCards() {
        var result = GetItems();        

        foreach (var player in Players) {
            // characters
            result.Add(player.Character);

            // curses
            result.AddRange(player.Curses);
        }


        // monsters
        foreach (var slot in MonsterSlots)
            if (slot.Card is not null)
                result.Add(slot.Card);

        // rooms
        foreach (var slot in RoomSlots)
            if (slot.Card is not null)
                result.Add(slot.Card);

        return result;
    }

    public List<InPlayMatchCard> GetMonsters() {
        // TODO? more
        var result = new List<InPlayMatchCard>();

        foreach (var slot in MonsterSlots)
            if (slot.Card is not null && slot.Card.Card.Template.Type == "Monster")
                result.Add(slot.Card);

        return result;
    }

    public async Task ExpandMonsterSlots(int amount) {
        LogInfo($"Expanding monster slots by {amount}");
        for (int i = 0; i < amount; i++) {
            var slot = new MonsterSlot(MonsterDeck, MonsterSlots.Count);
            await slot.Fill();
            MonsterSlots.Add(slot);           
        }
    }

}


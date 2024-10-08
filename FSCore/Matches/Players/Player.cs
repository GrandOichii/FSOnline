using Microsoft.VisualBasic;

namespace FSCore.Matches.Players;

/// <summary>
/// Participant of a match
/// </summary>
public class Player : IStateModifier {
    /// <summary>
    /// Setup-list of actions a player can make
    /// </summary>
    private static readonly List<IAction> ACTIONS = [
        new PassAction(),
        new PlayLootAction(),
        new ActivateAction(),
        new DeclarePurchaseAction(),
        new DeclareAttackAction(),
    ];

    /// <summary>
    /// Player action index
    /// </summary>
    private static readonly Dictionary<string, IAction> ACTION_MAP = [];

    static Player() {
        foreach (var action in ACTIONS) {
            ACTION_MAP.Add(action.ActionWord(), action);
        }
    }

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
    /// <summary>
    /// Amount of coins the player has
    /// </summary>
    public int Coins { get; private set; }
    /// <summary>
    /// Amount of loot cards played this turn
    /// </summary>
    public int LootPlayed { get; private set; }
    /// <summary>
    /// Amount of treasure cards a player can purchase
    /// </summary>
    public int PurchaseOpportunities { get; set; } // TODO change to private set;
    public int AttackOpportunities { get; set; } // TODO change to private set;
    /// <summary>
    /// Player's hand
    /// </summary>
    public List<HandMatchCard> Hand { get; }
    /// <summary>
    /// Character card
    /// </summary>
    public CharacterMatchCard Character { get; }
    /// <summary>
    /// Controlled items
    /// </summary>
    public List<OwnedInPlayMatchCard> Items { get; }
    /// <summary>
    /// State
    /// </summary>
    public PlayerState State { get; private set; }
    /// <summary>
    /// Player's soul cards
    /// </summary>
    public List<SoulCard> Souls { get; }
    /// <summary>
    /// History of rolls performed by the player (resets after each turn)
    /// </summary>
    public List<int> RollHistory { get; set; }
    /// <summary>
    /// Amount of damage marked on the player
    /// </summary>
    public Stats Stats { get; }
    public List<LuaFunction> DeathPreventors { get; }
    public List<LuaFunction> AtEndOfTurnEffects { get; } = [];
    public List<OwnedInPlayMatchCard> Curses { get; } = [];

    /// <summary>
    /// Name of the player that will be used for system logging
    /// </summary>
    public string LogName => $"{Name} [{Idx}]";

    public Player(Match match, string name, int idx, CardTemplate characterTemplate, IPlayerController controller) {
        Match = match;
        Name = name;
        Idx = idx;
        Controller = controller;

        Character = new(match, this, characterTemplate);
        Hand = [];
        Items = [];
        Souls = [];
        RollHistory = [];
        Stats = new();
        DeathPreventors = [];

        // Initial state
        State = new(this);
    }

    public int SoulCount() {
        var result = State.AdditionalSoulCount;
        foreach (var card in Souls)
            result += card.GetSoulValue();
        return result;
    }

    public bool Wins() {
        var count = SoulCount();

        // TODO some cards modify required soul count to win
        return count >= Match.Config.SoulsToWin;
    }

    /// <summary>
    /// Sets up the player
    /// </summary>
    public async Task Setup() {
        await Controller.Setup(Match, Idx);

        if (Match.Config.CharactersStartTapped)
            Character.Tapped = true;

        await AddStartingItems();

        await LootCards(
            Match.Config.InitialDealLoot,
            LootReasons.InitialDeal(Match.LState),
            LootSource.LOOT_DECK
        );
        await GainCoins(Match.Config.InitialDealCoins);
    }

    /// <summary>
    /// Cleans up the player
    /// </summary>
    public async Task CleanUp() {
        await Controller.CleanUp(Match, Idx);
    }

    /// <summary>
    /// Make player draw loot cards
    /// </summary>
    /// <param name="amount">Initial amount to be drawn</param>
    /// <param name="reason">Loot reason</param>
    /// <param name="source">Loot source</param>
    /// <returns>Resulting amount of cards drawn by the player</returns>
    /// <exception cref="MatchException"></exception>
    public async Task<int> LootCards(int amount, LuaTable reason, LootSource source = LootSource.DETERMINE) {
        if (source == LootSource.DETERMINE)
            source = LootSource.LOOT_DECK;

        try {
            foreach (var modifier in State.LootAmountModifiers) {
                var returned = modifier.Call(this, amount, reason);
                amount = LuaUtility.GetReturnAsInt(returned);
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to call loot amount modification function for player {LogName}", e);
        }

        return source switch
        {
            LootSource.LOOT_DECK => await LootFromLootDeck(amount),
            LootSource.LOOT_DISCARD => await LootFromLootDiscard(amount),
            _ => throw new MatchException($"unknown loot source: {source}"),
        };
    }

    /// <summary>
    /// Makes the player draw loot cards from the top of the loot deck
    /// </summary>
    /// <param name="amount">Amount of cards to be drawn</param>
    /// <returns>Amount of cards drawn by the player</returns>
    private async Task<int> LootFromLootDeck(int amount) {
        Match.LogDebug("Player {PlayerLogName} loots {Amount} cards from the loot deck", LogName, amount);
        var cards = Match.RemoveCardsFromTopOfLootDeck(amount);
        foreach (var card in cards) {
            await AddToHand(card);
        }

        return cards.Count;
    }

    /// <summary>
    /// Makes the player draw loot cards from the top of the loot discard
    /// </summary>
    /// <param name="amount">Amount of cards to be drawn</param>
    /// <returns>Amount of cards drawn by the player</returns>
    private async Task<int> LootFromLootDiscard(int amount) {
        // TODO

        return 0;
    }

    #region Hand

    /// <summary>
    /// Adds a loot card to the player's hand
    /// </summary>
    /// <param name="card">Loot card</param>
    public async Task AddToHand(MatchCard card) {
        var handCard = new HandMatchCard(card, this);
        Hand.Insert(0, handCard);

        // TODO? change card zone
        // TODO trigger (not draw)

        Match.LogDebug("Card {CardLogName} was put into hand of player {PlayerLogName}", card.LogName, LogName);
        // TODO add update
    }

    /// <summary>
    /// Returns the card with the specified ID
    /// </summary>
    /// <param name="id">Card ID</param>
    /// <returns>Hand card if found, else <c>null</c></returns>
    public HandMatchCard? HandCardOrDefault(string id) {
        return Hand.FirstOrDefault(c => c.Card.ID == id);
    }

    public bool RemoveFromHand(HandMatchCard card) {
        var removed = Hand.Remove(card);
        if (removed) {
            Match.LogDebug("Card {CardLogName} was removed from hand of player {PlayerLogName}", card.Card.LogName, LogName);
        }
        return removed;
    }

    public void ShouldRemoveFromHand(HandMatchCard card) {
        if (RemoveFromHand(card)) return;

        throw new MatchException($"Player {LogName} tried to remove card {card.Card.LogName} from hand, but failed");
    }

    #endregion

    private int ModCoinGain(int amount) {
        var initial = amount;
        try {
            foreach (var mod in State.CoinGainModifiers) {
                var returned = mod.Call(this, amount);
                amount = LuaUtility.GetReturnAsInt(returned);
            }
            return amount;
        } catch (Exception e) {
            throw new MatchException($"Failed to modify coin gain amount for player {LogName} (initial amount: {initial})", e);
        }
    }

    /// <summary>
    /// Gives the player coins
    /// </summary>
    /// <param name="amount">Initial amount of coins to be given to the player</param>
    /// <returns>Resulting amount of coins given to the player</returns>
    public async Task<int> GainCoins(int amount) {
        amount = ModCoinGain(amount);

        var taken = GainCoinsRaw(amount);

        return taken;
    }

    public int GainCoinsRaw(int amount) {
        var taken = Match.TakeCoins(amount);
        Coins += taken;
        Match.LogDebug("Player {LogName} gained {Taken} coins (expected to gain {Amount}), {CoinPool} left in coin pool", LogName, taken, amount, Match.CoinPool);

        // TODO emit trigger
        // TODO add to update
        return taken;
    }

    #region Recharging

    /// <summary>
    /// Untaps (recharges) all items the player has under their control
    /// </summary>
    public async Task UntapAll() {
        foreach (var card in GetInPlayCards()) {
            await card.Untap();
        }
    }

    #endregion

    #region Discard

    public async Task DiscardFromHand(int handIdx) {
        var card = Hand[handIdx];
        var removed = RemoveFromHand(card);
        if (!removed)
            throw new MatchException($"Failed to remove card {card.Card.LogName} from hand of player {LogName} (idx: {handIdx})");

        await Match.PlaceIntoDiscard(card.Card);

        // TODO add trigger
        // TODO add update
    }

    /// <summary>
    /// Forces the player to discard to hand size
    /// </summary>
    public async Task DiscardToHandSize() {
        // TODO modify max hand size
        var maxHandSize = Match.Config.MaxHandSize;

        while (Hand.Count > maxHandSize) {
            var options = new List<int>();
            for (int i = 0; i < Hand.Count; i++) options.Add(i);
            var idx = await ChooseCardInHand(options, $"Choose a card to discard (to hand size: {maxHandSize})");
            await DiscardFromHand(idx);
        }
    }

    #endregion

    /// <summary>
    /// Prompts the player to discard a room card (if they defeated a monster during their turn)
    /// </summary>
    public async Task PromptToDiscardRoom() {
        if (Match.DeadCards.Count == 0) return;

        // TODO could be better
        // TODO doesnt factor rooms that cant be discarded
        var options = Match.RoomSlots
            .Where(slot => slot.Card is not null)
            .Select(slot => slot.Card!.LogName)
            .ToList();

        // no rooms
        if (options.Count == 0) return;
        
        options.Add("-");
        var choice = await ChooseString(options, "Choose a room to discard");

        if (choice == "-") {
            Match.LogDebug("Player {PlayerLogName} declined to discard a room", LogName);
            return;
        }

        var slot = Match.RoomSlots
            .First(slot => slot.Card is not null && slot.Card.LogName == choice);

        await Match.DiscardFromPlay(slot.Card!);
    }

    public void ResetLootPlays() {
        LootPlayed = 0;
    }

    public void AddPurchaseOpportunitiesForTurn() {
        AddPurchaseOpportunities(Match.Config.PurchaseCountDefault);
    }
    
    public void AddAttackOpportunitiesForTurn() {
        AddAttackOpportunities(Match.Config.AttackCountDefault);
    }

    public void AddPurchaseOpportunities(int amount) {
        if (PurchaseOpportunities < 0) return;
        PurchaseOpportunities += amount;
    }

    public void AddAttackOpportunities(int amount) {
        if (AttackOpportunities < 0) return;
        AttackOpportunities += amount;
    }

    public void RemovePurchaseOpportunities() {
        PurchaseOpportunities = 0;
    }

    public void RemoveAttackOpportunities() {
        AttackOpportunities = 0;
    }

    /// <summary>
    /// Checks, whether the player can play a loot card from their hand
    /// </summary>
    /// <param name="card">Hand card</param>
    /// <returns>True, if none of the play checks failed</returns>
    public bool CanPlay(HandMatchCard card) {
        if (!card.CanPlay(this)) return false;

        return State.UnlimitedLootPlays || LootPlayed < State.LootPlaysForTurn;
    }

    /// <summary>
    /// Pay costs for playing the loot card
    /// </summary>
    /// <param name="card">Loot card</param>
    /// <returns></returns>
    public async Task<bool> PayCostsToPlay(HandMatchCard card, StackEffect effect) {
        // TODO additional costs

        var payed = card.PayCosts(effect);
        if (!payed) {
            return false;
        }

        LootPlayed++;

        return true;
    }

    /// <summary>
    /// Attempts to play a loot card from hand
    /// </summary>
    /// <param name="card">Hand card</param>
    /// <returns>True if successfully played the loot card</returns>
    public async Task<bool> TryPlayCard(HandMatchCard card) {
        if (!CanPlay(card)) return false;

        var stackEffect = new LootCardStackEffect(Match, Idx, card.Card);
        await Match.PlaceOnStack(stackEffect);

        var payed = await PayCostsToPlay(card, stackEffect);

        if (!payed) {
            Match.RemoveTopOfStack();
            Match.LogDebug("Player {PlayerLogName} decided not to pay costs for loot card {CardLogName}", LogName, card.Card.LogName);
            return false;
        }

        Match.LogDebug("Player {PlayerLogName} played loot card {CardLogName}", LogName, card.Card.LogName);

        ShouldRemoveFromHand(card);

        // TODO add update

        return true;
    }

    #region In-play cards

    public async Task GainControl(TreasureSlot slot) {
        var card = slot.Card
            ?? throw new MatchException($"Player {LogName} tried to gain control of treasure slot item at index [{slot.Idx}], which is empty");

        var newCard = new OwnedInPlayMatchCard(card, this);

        await Match.Emit("purchase", new() {
            { "Player", this },
            { "Card", newCard },
        });
        await slot.Fill();

        await Match.PlaceOwnedCard(newCard, false);
    }

    public async Task<List<OwnedInPlayMatchCard>> GainTreasure(int amount) {
        // TODO calculate actual amount

        return await GainTreasureRaw(amount);
    }

    public async Task<List<OwnedInPlayMatchCard>> GainTreasureRaw(int amount) {
        var cards = Match.RemoveCardsFromTopOfTreasureDeck(amount);

        var result = new List<OwnedInPlayMatchCard>();
        
        foreach (var card in cards) {
            var c = new OwnedInPlayMatchCard(card, this);
            await Match.PlaceOwnedCard(c);
            result.Add(c);
        }

        return result;
    }

    public async Task RemoveItem(OwnedInPlayMatchCard card) {
        var removed = Items.Remove(card);
        if (!removed)
            throw new MatchException($"Failed to remove item {card.LogName} from player {LogName}");
        
        Match.LogDebug("Item {CardLogName} was removed from player {PlayerLogName}", card.LogName, LogName);
        // TODO add to update
    }

    public async Task RemoveFromPlay(OwnedInPlayMatchCard card) {
        var removed = Items.Remove(card);
        if (removed) return;

        removed = Curses.Remove(card);
        if (removed) {
            Match.LogDebug("Curse {CardLogName} was removed from player {PlayerLogName}", card.LogName, LogName);
            return;
        }

        // TODO add to update
        throw new MatchException($"Failed to remove in-play card {card.LogName} from player {LogName}");
    }

    /// <summary>
    /// Gain control of item
    /// </summary>
    /// <param name="card">Item card</param>
    public async Task GainItem(OwnedInPlayMatchCard card) {
        // TODO add to update

        Items.Add(card);

        Match.LogDebug("Player {PlayerLogName} gained item {CardLogName}", LogName, card.LogName);

        await Match.Emit("item_gain", new() {
            { "player", this },
            { "item", card }
        });
    }

    public async Task LoseItem(OwnedInPlayMatchCard card) {
        await RemoveItem(card);

        // TODO trigger
    }

    public OwnedInPlayMatchCard? GetInPlayCardOrDefault(string ipid) {
        return GetInPlayCards().FirstOrDefault(c => c.IPID == ipid);
    }

    public List<OwnedInPlayMatchCard> GetInPlayCards() {
        var result = new List<OwnedInPlayMatchCard>(Items) {
            Character
        };
        result.AddRange(Curses);

        return result;
    }

    /// <summary>
    /// Add starting items of the character
    /// </summary>
    public async Task AddStartingItems() {
        Match.LogDebug("Adding Starting Items for player {PlayerLogName}", LogName);
        await Match.CreateStartingItems(this, Character);
    }

    #endregion

    public void PayCoins(int amount) {
        LoseCoins(amount);
        if (Coins < 0)
            throw new MatchException($"Player {LogName} payed {amount} coins, which resulted in their balance being equal to {Coins}");

        Match.LogDebug("Player {PlayerLogName} payed {Amount} coins", LogName, amount);

        // TODO add update
    }

    #region Purchasing

    public async Task DeclarePurchase() {
        // TODO is this supposed to be here or during resolution
        PurchaseOpportunities--;

        var effect = new DeclarePurchaseStackEffect(Match, Idx);

        Match.LogDebug("Player {LogName} declares a purchase", LogName);
        await Match.PlaceOnStack(effect);

        // TODO trigger
    }

    /// <summary>
    /// Get the cost of buying the item at the designated shop slot
    /// </summary>
    /// <param name="slot">Shop slot, -1 for top of treasure deck</param>
    /// <returns>The item's cost</returns>
    public int CostOfSlot(int slot) {
        // TODO catch exceptions
        var result = Match.Config.PurchaseCost;

        try {
            foreach (var mod in State.PurchaseCostModifiers) {
                var returned = mod.Call(slot, result);
                result = LuaUtility.GetReturnAsInt(returned);
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to execute coin gain amount modification function for player {LogName}", e);
        }
        
        return result;
    }

    public bool TryPayCoinsForSlot(int slot) {
        var cost = CostOfSlot(slot);
        if (Coins < cost) return false;

        PayCoins(cost);
        return true;
    }

    public List<int> AvailableToPurchase() {
        var indicies = new List<int>();
        if (Match.TreasureDeck.Size > 0) indicies.Add(-1);

        indicies.AddRange(Match.TreasureSlots.Select(slot => slot.Idx));

        var result = new List<int>();
        foreach (var idx in indicies) {
            if (idx > 0 && Match.TreasureSlots[idx].Card is null) continue;
            if (Coins < CostOfSlot(idx)) continue;
            result.Add(idx);
        }

        return result;
    }

    #endregion

    public int LoseCoinsRaw(int amount) {
        if (Coins < amount)
            amount = Coins;
            
        Coins -= amount;
        return amount;
    }

    public int LoseCoins(int amount) {
        var lost = LoseCoinsRaw(amount);
        Match.AddToCoinPool(lost);
        return lost;
    }

    public void Modify(ModificationLayer layer)
    {
        // hand
        foreach (var card in Hand)
            card.Modify(layer);

        // in-play cards
        var cards = GetInPlayCards();
        foreach (var card in cards)
            card.Modify(layer);
    }

    public void UpdateState()
    {
        State = new(this);
        Stats.UpdateState(this);

        // hand
        foreach (var card in Hand)
            card.UpdateState();

        // in-play cards
        var cards = GetInPlayCards();
        foreach (var card in cards)
            card.UpdateState();
    }

    public async Task<string> PromptAction(List<string> options) {
        while (true) {
            var result = await Controller.PromptAction(Match, Idx, options);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking action - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }

    #region Choice

    /// <summary>
    /// Prompts the user a string from a list of options and checks it's validity
    /// </summary>
    /// <param name="options">List of string options</param>
    /// <param name="hint">Hint</param>
    /// <returns>The picked valid string</returns>
    /// <exception cref="MatchException"></exception>
    public async Task<string> ChooseString(List<string> options, string hint) {
        while (true) {
            var result = await Controller.ChooseString(Match, Idx, options, hint);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking option - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }

    /// <summary>
    /// Prompts the user an item choice from a list of options and checks it's validity
    /// </summary>
    /// <param name="options">List of item in-play IDs</param>
    /// <param name="hint">Hint</param>
    /// <returns>The picked valid IPID</returns>
    /// <exception cref="MatchException"></exception>
    public async Task<string> ChooseItem(List<string> options, string hint) {
        while (true) {
            var result = await Controller.ChooseItem(Match, Idx, options, hint);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking item IPID - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }

    public async Task<int> ChoosePlayer(List<int> options, string hint, bool optional=false) {
        while (true) {
            var result = await Controller.ChoosePlayer(Match, Idx, options, hint);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking player - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }
    public async Task<(TargetType, string)> ChooseMonsterOrPlayer(List<string> ipids, List<int> indicies, string hint, bool optional=false) {
        while (true) {
            var (type, value) = await Controller.ChooseMonsterOrPlayer(Match, Idx, ipids, indicies, hint);

            switch (type) {
            case TargetType.PLAYER:
                if (!indicies.Contains(int.Parse(value))) {
                    if (Match.Config.StrictMode)
                        throw new MatchException($"Invalid choice for picking monster/player - {value} (player: {LogName})");
                    continue;
                }
                break;
            case TargetType.ITEM:
                if (!ipids.Contains(value)) {
                    if (Match.Config.StrictMode)
                        throw new MatchException($"Invalid choice for picking monster/player - {value} (player: {LogName})");
                    continue;
                }
                break;
            default:
                throw new MatchException($"Invalid out TargetType for choosing monster/player: {type}");
            }

            return (type, value);
        }
    }
    public async Task<string> ChooseStackEffect(List<string> options, string hint, bool optional=false) {
        while (true) {
            var result = await Controller.ChooseStackEffect(Match, Idx, options, hint);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking stack effect - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }
    
    public async Task<int> ChooseCardInHand(List<int> options, string hint, bool optional=false) {
        while (true) {
            var result = await Controller.ChooseCardInHand(Match, Idx, options, hint);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking card in hand - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }

    public async Task<int> ChooseItemToPurchase() {
        var options = AvailableToPurchase();

        while (true) {
            var result = await Controller.ChooseItemToPurchase(Match, Idx, options);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking slot index to purchase - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }

    public async Task<DeckType> ChooseDeck(List<DeckType> options, string hint, bool optional=false) {
        // TODO use optional
        while (true) {
            var result = await Controller.ChooseDeck(Match, Idx, options, hint);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking dekc - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }

    #endregion

    public async Task UpdateController() {
        await Controller.Update(Match, Idx);
    }

    public async Task PerformAction() {
        var options = new List<string>();
        foreach (var a in ACTION_MAP.Values)
            options.AddRange(a.GetAvailable(Match, Idx));
        
        if (options.Count == 0) {
            throw new MatchException($"Player {LogName} doens't have any available actions");
        }
        var action = await PromptAction(options);

        var words = action.Split(" ");
        var actionWord = words[0];
        if (!ACTION_MAP.ContainsKey(actionWord)) {
            if (!Match.Config.StrictMode) {
                Match.LogWarning($"Unknown action word from player {LogName}: {actionWord}");
                return;
            }

            throw new UnknownActionException($"Unknown action from player {LogName}: {action}");
        }

        await ACTION_MAP[actionWord].Exec(Match, Idx, words);
    }

    #region Souls

    public async Task AddSoulCard(SoulCard card) {
        // TODO check if can gain souls

        Souls.Add(card);
        Match.LogDebug("Player {PlayerLogName} gained soul card {CardLogName}", LogName, card.Original.LogName);

        await Match.Emit("soul_enter", new() {
            { "Owner", this },
            { "Card", card },
        });

        // TODO update
    }

    #endregion

    #region Damage

    public async Task DamageToPlayerRequest(int toIdx, int amount, StackEffect damageSource) {
        var effect = new DamageStackEffect(Match, Idx, amount, damageSource, Match.GetPlayer(toIdx));
        await Match.PlaceOnStack(effect);
    }

    public async Task LoseHealth(int amount, StackEffect source) {
        Stats.Damage += amount;
        if (Stats.Damage >= Stats.State.Health) {
            Stats.Damage = Stats.State.Health;
        }

        Stats.CheckDead(source);

        Match.LogDebug("Player {LogName} lost {Amount} health", LogName, amount);

        // TODO update
        // TODO? trigger
    }

    public void HealToMax() {
        Stats.Damage = 0;
        Stats.IsDead = false;
        Stats.DeathSource = null;
    }

    public async Task ProcessDamage(int amount, DamageStackEffect effect) {
        await Stats.ProcessDamage(this, amount, effect);
    }

    #endregion

    #region Death

    public void AddDeathPreventor(LuaFunction preventorFunc) {
        DeathPreventors.Add(preventorFunc);
    }

    public async Task PushDeath(StackEffect deathSource) {
        Match.LogDebug("Death of player {LogName} is pushed onto the stack", LogName);

        var effect = new PlayerDeathStackEffect(this, deathSource);
        await Match.PlaceOnStack(effect);

        await Match.Emit("player_death_before_penalties", new() {
            { "Player", this },
            { "Source", deathSource },
        });

    }

    public async Task CheckDead() {
        if (Stats.DeathSource is null) return;

        // dead
        await PushDeath(Stats.DeathSource);

        Stats.DeathSource = null;
    }

    public async Task ProcessDeath(StackEffect deathSource) {
        if (Stats.IsDead) return;

        // TODO catch exceptions
        foreach (var preventor in DeathPreventors) {
            var returned = preventor.Call(deathSource);
            if (LuaUtility.GetReturnAsBool(returned)) {
                Match.LogDebug("Death of player {LogName} was prevented", LogName);
                DeathPreventors.Remove(preventor);
                return;
            }
        }

        // TODO death replacement effects
        if (Match.CurPlayerIdx == Idx) {
            Match.TurnEnded = true;
        }
        Stats.IsDead = true;

        await PayDeathPenalty(deathSource);

        Match.LogDebug("Player {LogName} dies", LogName);
        // TODO fizzle all DeclarePurchaseStackEffects

        await Match.Emit("player_death", new() {
            { "Player", this },
            { "Source", deathSource },
        });
        // TODO add update
    }

    public int GetDeathPenaltyCoinLoss() {
        var result = Match.Config.DeathPenaltyCoins;
        try {
            foreach (var mod in State.DeathPenaltyCoinLoseAmountModifiers) {
                var returned = mod.Call(result);
                result = LuaUtility.GetReturnAsInt(returned);
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to execute death penalty coin loss replacement effects for player {LogName}", e);
        }
        return result;
    }

    public int GetDeathPenaltyLootDiscardAmount() {
        var result = Match.Config.DeathPenaltyLoot;
        
        try {
            foreach (var mod in State.DeathPenaltyCoinLoseAmountModifiers) {
                var returned = mod.Call(result);
                result = LuaUtility.GetReturnAsInt(returned);
            }
        } catch (Exception e) {
            throw new MatchException($"Failed to execute death penalty coin loss replacement effects for player {LogName}", e);
        }

        return result;
    }

    public int GetDeathPenaltyDestroyedItemsAmount() {
        var result = Match.Config.DeathPenaltyItems;

        // TODO

        return result;
    }

    public async Task PayDeathPenalty(StackEffect deathSource) {
        // TODO? move this to a Lua script
        // death penalty replacement effects
        foreach (var effect in State.DeathPenaltyReplacementEffects) {
            var returned = effect.Call(this, deathSource);
            if (LuaUtility.GetReturnAsBool(returned)) return;
        }

        var itemsAmount = GetDeathPenaltyDestroyedItemsAmount();
        for (int i = 0; i < itemsAmount; i++) {
            var ipids = Items.Where(item => !item.HasLabel("Eternal")).Select(item => item.IPID).ToList();
            if (ipids.Count == 0) break;
            var ipid = await ChooseItem(ipids, "Choose an item to destroy (for death penalty)");
            await Match.DestroyItem(ipid);
        }

        var lootDiscardAmount = GetDeathPenaltyLootDiscardAmount();
        for (int i = 0; i < lootDiscardAmount; i++) {
            if (Hand.Count == 0) break;

            var options = new List<int>();
            for (int hi = 0; hi < Hand.Count; hi++) options.Add(hi);
            var idx = await ChooseCardInHand(options, $"Choose a card to discard (for death penalty)");
            await DiscardFromHand(idx);
        }

        LoseCoins(GetDeathPenaltyCoinLoss());

        foreach (var item in GetInPlayCards()) {
            // TODO check if item has an activated ability with the "Tap" label
            await item.Tap();
        }
    }

    #endregion

    public bool Remove(string id) {
        var card = Hand.FirstOrDefault(c => c.Card.ID == id);
        if (card is not null) {
            Hand.Remove(card);
            return true;
        }

        var iCard = Items.FirstOrDefault(c => c.Card.ID == id);
        if (iCard is not null) {
            Items.Remove(iCard);
            return true;
        }

        var sCard = Souls.FirstOrDefault(c => c.Original.ID == id);
        if (sCard is not null) {
            Souls.Remove(sCard);
            return true;
        }

        var cCard = Curses.FirstOrDefault(c => c.Card.ID == id);
        if (cCard is not null) {
            Curses.Remove(cCard);
            return true;
        }
        
        return false;
    }

    #region Damage prevention

    public int PreventableDamage() {
        return Stats.DamagePreventors.Count;
    }

    public async Task AddDamagePreventors(int amount) {
        Stats.AddDamagePreventors(amount);
        // TODO? update
    }

    #endregion

    #region Attacking

    public async Task DeclareAttack() {
        // TODO is this supposed to be here or during resolution
        AttackOpportunities--;

        var effect = new DeclareAttackStackEffect(Match, Idx);

        Match.LogDebug("Player {LogName} declares an attack", LogName);
        await Match.PlaceOnStack(effect);

        // TODO trigger
    }

    /// <summary>
    /// Gets all monster slot indicies that can be attacked by the player
    /// </summary>
    /// <returns>List of monster slot indicies + -1 if can attack the top of the monster deck</returns>
    public List<int> AvailableToAttack() {
        var result = new List<int>();

        // top of monster deck
        result.Add(-1);

        // monster card slots
        foreach (var slot in Match.MonsterSlots)
            if (slot.Card is not null && slot.Card.Card.Template.Type == "Monster")
                result.Add(slot.Idx);
        return result;
    }


    public async Task<int> ChooseMonsterToAttack() {
        var options = AvailableToAttack();

        while (true) {
            var result = await Controller.ChooseMonsterToAttack(Match, Idx, options);

            if (!options.Contains(result)) {
                if (Match.Config.StrictMode)
                    throw new MatchException($"Invalid choice for picking slot index to purchase - {result} (player: {LogName})");
                continue;
            }

            return result;
        }
    }

    public async Task AttackMonsterInSlot(int slot) {
        var monster = Match.MonsterSlots[slot].Card
            ?? throw new MatchException($"Player {LogName} tried to attack monster slot {slot}, where there are no monsters")
        ;       

        var attack = new AttackStackEffect(Match, Idx, monster);
        await Match.PlaceOnStack(attack);
    }

    #endregion   

    public int GetAttack() => Stats.State.Attack;

    public int CalculateRollResult(RollStackEffect stackEffect) {
        var result = stackEffect.Value;       

        // TODO catch exceptions
        foreach (var mod in State.RollResultModifiers) {
            var returned = mod.Call(result, stackEffect);
            result = LuaUtility.GetReturnAsInt(returned);
        }
        return Math.Clamp(result, 1, 6);
    }
}
using System.Runtime.CompilerServices;

namespace FSCore.Matches.Players;

/// <summary>
/// Participant of a match
/// </summary>
public class Player : IStateModifier {
    /// <summary>
    /// Setup-list of actions a player can make
    /// </summary>
    private static readonly List<IAction> ACTIONS = new() {
        new PassAction(),
        new PlayLootAction(),
        new ActivateAction(),
        new DeclarePurchaseAction(),
    };

    /// <summary>
    /// Player action index
    /// </summary>
    private static readonly Dictionary<string, IAction> ACTION_MAP = new(){};

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
    /// Amount of loot cards the player can play for this turn
    /// </summary>
    public int LootPlays { get; private set; }
    /// <summary>
    /// Amount of treasure cards a player can purchase
    /// </summary>
    public int PurchaseOpportunities { get; set; } // TODO change to private set;
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
    public int Damage { get; set; }
    public bool IsDead { get; set; }
    public StackEffect? DeathSource { get; private set; }

    /// <summary>
    /// Name of the player that will be used for system logging
    /// </summary>
    public string LogName => Name + $" [{Idx}]";

    public Player(Match match, string name, int idx, CharacterCardTemplate characterTemplate, IPlayerController controller) {
        Match = match;
        Name = name;
        Idx = idx;
        Controller = controller;

        Character = new(match, this, characterTemplate);
        Hand = new();
        Items = new();
        Souls = new();
        RollHistory = new();
        IsDead = false;
        DeathSource = null;

        // Initial state
        State = new(this);
    }

    public int SoulCount() {
        // TODO other effects
        var result = 0;
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
        // TODO catch exception

        if (source == LootSource.DETERMINE)
            source = LootSource.LOOT_DECK;

        foreach (var modifier in State.LootAmountModifiers) {
            var returned = modifier.Call(this, amount, reason);
            amount = LuaUtility.GetReturnAsInt(returned);
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

        Match.LogInfo($"Card {card.LogName} was put into hand of player {LogName}");
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
        return Hand.Remove(card);
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

        var taken = Match.TakeCoins(amount);
        GainCoinsRaw(taken);

        Match.LogInfo($"Player {LogName} gained {taken} coins (expected to gain {amount}), {Match.CoinPool} left in coin pool");

        // TODO emit trigger
        // TODO add to update

        return taken;
    }

    public void GainCoinsRaw(int amount) {
        Coins += amount;
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
        // TODO
    }

    /// <summary>
    /// Adds a number of loot plays for the turn
    /// </summary>
    public void AddLootPlayForTurn() {
        AddLootPlay(State.LootPlaysForTurn);
    }

    /// <summary>
    /// Adds loot plays
    /// </summary>
    /// <param name="amount">Amount of loot plays</param>
    public void AddLootPlay(int amount) {
        if (LootPlays < 0) return;
        LootPlays += amount;
    }

    public void AddPurchaseOpportunitiesForTurn() {
        AddPurchaseOpportunities(Match.Config.PurchaseCountDefault);
    }

    public void AddPurchaseOpportunities(int amount) {
        if (PurchaseOpportunities < 0) return;
        PurchaseOpportunities += amount;
    }

    /// <summary>
    /// Removes all loot plays from the player
    /// </summary>
    public void RemoveLootPlays() {
        LootPlays = 0;
    }

    public void RemovePurchaseOpportunities() {
        PurchaseOpportunities = 0;
    }

    /// <summary>
    /// Checks, whether the player can play a loot card from their hand
    /// </summary>
    /// <param name="card">Hand card</param>
    /// <returns>True, if none of the play checks failed</returns>
    public bool CanPlay(HandMatchCard card) {
        if (!card.CanPlay(this)) return false;

        return LootPlays < 0 || card.State.LootCost <= LootPlays;
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

        if (LootPlays > 0) {
            LootPlays--;
            if (LootPlays < 0)
                throw new MatchException($"Unexpected scenario: player payed loot cost for card {card.Card.LogName}, which resulted in their loot plays being equal to {LootPlays}");
        }

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
            Match.LogInfo($"Player {LogName} decided not to pay costs for loot card {card.Card.LogName}");
            return false;
        }

        Match.LogInfo($"Player {LogName} played loot card {card.Card.LogName}");

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
        // TODO add to update
    }

    /// <summary>
    /// Gain control of item
    /// </summary>
    /// <param name="card">Item card</param>
    public async Task GainItem(OwnedInPlayMatchCard card) {
        // TODO trigger
        // TODO add to update

        Items.Add(card);

        Match.LogInfo($"Player {LogName} gained item {card.LogName}");
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
        return result;
    }

    /// <summary>
    /// Add starting items of the character
    /// </summary>
    public async Task AddStartingItems() {
        var items = await Match.CreateStartingItems(this, Character.GetTemplate());
        foreach (var item in items) {
            await GainItem(item);
        }
    }

    #endregion

    public void PayCoins(int amount) {
        Coins -= amount;
        if (Coins < 0)
            throw new MatchException($"Player {LogName} payed {amount} coins, which resulted in their balance being equal to {Coins}");

        Match.AddToCoinPool(amount);

        // TODO add update
    }

    #region Shops

    /// <summary>
    /// Get the cost of buying the item at the designated shop slot
    /// </summary>
    /// <param name="slot">Shop slot, -1 for top of treasure deck</param>
    /// <returns>The item's cost</returns>
    public int CostOfSlot(int slot) {
        // TODO catch exceptions
        var result = Match.Config.PurchaseCost;

        foreach (var mod in State.PurchaseCostModifiers) {
            var returned = mod.Call(slot, result);
            result = LuaUtility.GetReturnAsInt(returned);
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

    public void LoseCoins(int amount) {
        Coins -= amount;
        if (Coins < 0)
            Coins = 0;
        Match.AddToCoinPool(amount);
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
        Match.LogInfo($"Player {LogName} gained soul card {card.Original.LogName}");

        await Match.Emit("soul_enter", new() {
            { "Owner", this },
            { "Card", card },
        });
        
        // TODO update
    }

    #endregion

    #region Damage

    public void HealToMax() {
        Damage = 0;
        IsDead = false;
        DeathSource = null;
    }

    public async Task ProcessDamage(int amount, StackEffect source) {
        // TODO calculate whether damage is prevented
        // TODO calculate the final amount of damage
        // TODO use source

        Match.LogInfo($"Player {LogName} was dealt {amount} damage");
        Damage += amount;
        if (Damage >= State.Stats.Health) {
            Damage = State.Stats.Health;
            DeathSource = source;
        }

        await Match.Emit("player_damaged", new() {
            { "Player", this },
            { "Amount", amount },
            { "Source", source },
        });
    }

    #endregion

    #region Death

    public async Task PushDeath(StackEffect deathSource) {
        Match.LogInfo($"Death of player {LogName} is pushed onto the stack");

        var effect = new PlayerDeathStackEffect(this, deathSource);
        await Match.PlaceOnStack(effect);
    }

    public async Task CheckDead() {
        if (DeathSource is null) return;

        // dead
        await PushDeath(DeathSource);

        // TODO feels like this shouldn't be here
        await Match.Emit("player_death_before_penalties", new() {
            { "Player", this },
            { "Source", DeathSource },
        });

        DeathSource = null;
    }

    public async Task ProcessDeath(StackEffect deathSource) {
        if (IsDead) return;

        // TODO death replacement effects
        if (Match.CurPlayerIdx == Idx) {
            Match.TurnEnded = true;
        }
        IsDead = true;

        await PayDeathPenalty(deathSource);

        Match.LogInfo($"Player {LogName} dies");
        // TODO death penalty
        // TODO fizzle all DeclarePurchaseStackEffects

        // TODO trigger
        await Match.Emit("player_death", new() {
            { "Player", this },
            { "Source", deathSource },
        });
        // TODO add update
    }

    public async Task PayDeathPenalty(StackEffect deathSource) {
        // TODO? move this to a Lua script
        // TODO death penalty replacement effects

        for (int i = 0; i < Match.Config.DeathPenaltyItems; i++) {
            var ipids = Items.Where(item => !item.HasLabel("Eternal")).Select(item => item.IPID).ToList();
            if (ipids.Count == 0) break;
            var ipid = await ChooseItem(ipids, "Choose an item to destroy (for death penalty)");
            await Match.DestroyItem(ipid);
        }

        // TODO modify the amount of loot cards discarded
        for (int i = 0; i < Match.Config.DeathPenaltyLoot; i++) {
            if (Hand.Count == 0) break;

            var options = new List<int>();
            for (int hi = 0; hi < Hand.Count; hi++) options.Add(hi);
            var idx = await ChooseCardInHand(options, $"Choose a card to discard (for death penalty)");
            await DiscardFromHand(idx);
        }

        // TODO modify the amount of coins lost
        LoseCoins(Match.Config.DeathPenaltyCoins);

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

        return false;
    }
}
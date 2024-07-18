using System.Runtime.CompilerServices;

namespace FSCore.Matches.Players;

/// <summary>
/// Participant of a match
/// </summary>
public class Player : IStateModifier {
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
    /// Name of the player that will be used for system logging
    /// </summary>
    public string LogName => Name + $" [{Idx}]";

    public Player(Match match, string name, int idx, CharacterCardTemplate characterTemplate, IPlayerController controller) {
        Match = match;
        Name = name;
        Idx = idx;
        Controller = controller;

        Hand = new();
        Items = new();
        Character = new(match, this, characterTemplate);
        // Initial state
        State = new(this);
    }

    /// <summary>
    /// Sets up the player
    /// </summary>
    public async Task Setup() {
        await Controller.Setup(Match, Idx);

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
        // TODO modify the amount of cards to be looted
        // TODO determine loot source
        if (source == LootSource.DETERMINE)
            source = LootSource.LOOT_DECK;

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
        // TODO trigger

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
                System.Console.WriteLine("CALL");
                var returned = mod.Call(this, amount);
                System.Console.WriteLine("PARSE");
                System.Console.WriteLine(returned[0]);
                amount = LuaUtility.GetReturnAsInt(returned);
                System.Console.WriteLine("END");
            }
            return amount;
        } catch (Exception e) {
            throw new MatchException($"Failed to modify coin gain amount for player {LogName} (initial amount: {amount})");
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
        Coins += taken;

        Match.LogInfo($"Player {LogName} gained {taken} coins (expected to gain {amount}), {Match.CoinPool} left in coin pool");

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

    /// <summary>
    /// Forces the player to discard to hand size
    /// </summary>
    public async Task DiscardToHandSize() {
        // TODO
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
        AddLootPlay(Match.Config.LootPlay);
    }

    /// <summary>
    /// Adds loot plays
    /// </summary>
    /// <param name="amount">Amount of loot plays</param>
    public void AddLootPlay(int amount) {
        if (LootPlays < 0) return;
        LootPlays += amount;
    }

    /// <summary>
    /// Removes all loot plays from the player
    /// </summary>
    public void RemoveLootPlays() {
        LootPlays = 0;
    }

    /// <summary>
    /// Checks, whether the player can play a loot card from their hand
    /// </summary>
    /// <param name="card">Hand card</param>
    /// <returns>True, if none of the play checks failed</returns>
    public bool CanPlay(HandMatchCard card) {
        // TODO add more checks

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

    public async Task<List<OwnedInPlayMatchCard>> GainTreasure(int amount) {
        // TODO calculate actual amount

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
        Items.Remove(card);
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

    public void LoseCoins(int amount) {
        Coins -= amount;
        if (Coins < 0)
            Coins = 0;
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
}
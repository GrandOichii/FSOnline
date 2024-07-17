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
    /// Name of the player that will be used for system logging
    /// </summary>
    public string LogName => Name + $" [{Idx}]";

    public Player(Match match, string name, int idx, IPlayerController controller) {
        Match = match;
        Name = name;
        Idx = idx;
        Controller = controller;

        Hand = new();
    }

    /// <summary>
    /// Sets up the player
    /// </summary>
    public async Task Setup() {
        await Controller.Setup(Match, Idx);

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

    /// <summary>
    /// Gives the player coins
    /// </summary>
    /// <param name="amount">Initial amount of coins to be given to the player</param>
    /// <returns>Resulting amount of coins given to the player</returns>
    public async Task<int> GainCoins(int amount) {
        // TODO modify the actual amount using static state modifiers/replacement effects

        var taken = Match.TakeCoins(amount);
        Coins += taken;

        Match.LogInfo($"Player {LogName} gained {taken} coins (expected to gain {amount}), {Match.CoinPool} left in coin pool");

        // TODO emit trigger
        // TODO add to update

        return taken;
    }

    #region Recharging

    /// <summary>
    /// Recharges all items the player has under their control
    /// </summary>
    public async Task RechargeAll() {
        // TODO recharge character
        // TODO recharge items
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
        LootPlays += Match.Config.LootPlay;
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

        return card.State.LootCost <= LootPlays;
    }

    /// <summary>
    /// Pay costs for playing the loot card
    /// </summary>
    /// <param name="card">Loot card</param>
    /// <returns></returns>
    public async Task PayCostsToPlay(HandMatchCard card) {
        // TODO additional costs

        LootPlays--;
        if (LootPlays < 0)
            throw new MatchException($"Unexpected scenario: player payed loot cost for card {card.Card.LogName}, which resulted in their loot plays being equal to {LootPlays}");
    }

    /// <summary>
    /// Attempts to play a loot card from hand
    /// </summary>
    /// <param name="card">Hand card</param>
    /// <returns>True if successfully played the loot card</returns>
    public async Task<bool> TryPlayCard(HandMatchCard card) {
        if (!CanPlay(card)) return false;

        await PayCostsToPlay(card);

        ShouldRemoveFromHand(card);
        // TODO place loot card onto the stack

        await Match.PlaceOnStack(Idx, card);

        // TODO add update

        return true;
    }
}
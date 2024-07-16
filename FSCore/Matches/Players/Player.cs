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
    /// Name of the player that will be used for system logging
    /// </summary>
    public string LogName => Name + $" [{Idx}]";

    public Player(Match match, string name, int idx, IPlayerController controller) {
        Match = match;
        Name = name;
        Idx = idx;
        Controller = controller;
    }

    /// <summary>
    /// Sets up the player
    /// </summary>
    public async Task Setup() {
        await Controller.Setup(Match, Idx);

        await LootCards(
            Match.Config.InitialDealLoot,
            LootReasons.InitialDeal(Match.LState)
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
    public async Task<int> LootCards(int amount, LuaTable reason, LootSource source = LootSource.LOOT_DECK) {
        // TODO modify the amount of cards to be looted

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

    /// <summary>
    /// Adds a loot card to the player's hand
    /// </summary>
    /// <param name="card">Loot card</param>
    public async Task AddToHand(MatchCard card) {
        // TODO
        // TODO? change card zone

        Match.LogInfo($"Card {card.LogName} was put into hand of player {LogName}");
        // TODO add update
    }

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

    public async Task RechargeAll() {
        // TODO recharge character
        // TODO recharge items
    }

    #endregion

    #region Discard

    public async Task DiscardToHandSize() {
        // TODO
    }

    #endregion

    public async Task PromptToDiscardRoom() {
        // TODO
    }

    public void AddLootPlayForTurn() {
        LootPlays += Match.Config.LootPlay;
    }

    public void RemoveLootPlays() {
        LootPlays = 0;
    }
}
namespace FSCore.Tests.Setup;

public class MatchConfigBuilder
{
    private MatchConfig _result; // TODO set default
    private readonly LootDeckBuilder _lootBuilder;
    private readonly TreasureDeckBuilder _treasureBuilder;
    private readonly MonsterDeckBuilder _monsterBuilder;
    private readonly BonusSoulDeckBuilder _bonusSoulsBuilder;


    public MatchConfigBuilder()
    {
        _lootBuilder = new(this);
        _treasureBuilder = new(this);
        _monsterBuilder = new(this);
        _bonusSoulsBuilder = new(this);

        _result = new()
        {
            AttackCountDefault = 1,
            BonusSoulCount = 3,
            BonusSouls = [],
            Characters = [],
            CharactersStartTapped = true,
            CoinPool = 100,
            Curses = [],
            DeathPenaltyCoins = 1,
            DeathPenaltyItems = 1,
            DeathPenaltyLoot = 1,
            Events = [],
            ForceInclude3PlusCards = true,
            InitialDealCoins = 3,
            InitialDealLoot = 3,
            InitialMonsterSlots = 2,
            InitialRoomSlots = 1,
            InitialTreasureSlots = 2,
            LootCards = [],
            LootPlay = 1,
            LootStepLootAmount = 1,
            MaxHandSize = 10,
            MaxPlayerCount = 4,
            Monsters = [],
            PurchaseCost = 10,
            PurchaseCountDefault = 1,
            RandomFirstPlayer = true,
            Rooms = [],
            SoulsToWin = 4,
            StartingItems = [],
            StrictMode = true,
            Treasures = [],
            UseRooms = true,
            PromptWhenPayingCoins = false,
            PromptWhenPayingLife = false
        };
    }

    public LootDeckBuilder ConfigLootDeck() => _lootBuilder;
    public TreasureDeckBuilder ConfigTreasureDeck() => _treasureBuilder;
    public MonsterDeckBuilder ConfigMonsterDeck() => _monsterBuilder;
    public BonusSoulDeckBuilder ConfigBonusSouls() => _bonusSoulsBuilder;

    public MatchConfigBuilder InitialCoins(int amount)
    {
        _result.InitialDealCoins = amount;
        return this;
    }

    public MatchConfigBuilder DisableDeathPenalty()
    {
        _result.DeathPenaltyCoins = 0;
        _result.DeathPenaltyLoot = 0;
        _result.DeathPenaltyItems = 0;
        return this;
    }

    public MatchConfigBuilder InitialLoot(int amount)
    {
        _result.InitialDealLoot = amount;
        return this;
    }
    
    public MatchConfigBuilder InitialTreasureSlots(int amount)
    {
        _result.InitialTreasureSlots = amount;
        return this;
    }

    public MatchConfigBuilder InitialMonsterSlots(int amount)
    {
        _result.InitialMonsterSlots = amount;
        return this;
    }

    public MatchConfigBuilder LootStepLootAmount(int amount)
    {
        _result.LootStepLootAmount = amount;
        return this;
    }

    public MatchConfigBuilder LootPlay(int amount)
    {
        _result.LootPlay = amount;
        return this;
    }

    public MatchConfig Build()
    {
        _result.LootCards = _lootBuilder.Cards;
        _result.Treasures = _treasureBuilder.Cards;
        _result.Monsters = _monsterBuilder.Monsters;
        _result.BonusSouls = _bonusSoulsBuilder.Cards;
        // TODO build decks

        return _result;
    }
}

public class TreasureDeckBuilder(MatchConfigBuilder parent)
{
    public MatchConfigBuilder Done() => parent;

    public List<string> Cards { get; } = [];

    public TreasureDeckBuilder Add(string key)
    {
        // TODO? duplicates
        Cards.Add(key);
        return this;
    }
}

public class MonsterDeckBuilder(MatchConfigBuilder parent)
{
    public MatchConfigBuilder Done() => parent;

    public List<string> Monsters { get; } = [];

    public MonsterDeckBuilder AddMonster(string key)
    {
        // TODO? duplicates
        Monsters.Add(key);
        return this;
    }
}

public class BonusSoulDeckBuilder(MatchConfigBuilder parent)
{
    public MatchConfigBuilder Done() => parent;

    public List<string> Cards { get; } = [];

    public BonusSoulDeckBuilder Add(string key)
    {
        if (Cards.Contains(key))
        {
            throw new Exception($"Can't add duplicate bonus soul card (key: {key})");
        }

        Cards.Add(key);
        return this;
    }
}

public class LootDeckBuilder(MatchConfigBuilder parent)
{
    public MatchConfigBuilder Done() => parent;

    public Dictionary<string, int> Cards { get; } = [];

    public LootDeckBuilder Add(string card, int amount = 1)
    {
        if (!Cards.TryGetValue(card, out int value))
        {
            value = 0;
            Cards.Add(card, value);
        }
        Cards[card] = value + amount;

        return this;
    }
}
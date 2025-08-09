namespace FSCore.Tests.Setup;

public class MatchConfigBuilder {
    private readonly LootDeckBuilder _lootBuilder;

    public MatchConfigBuilder() {
        _lootBuilder = new(this);
    }

    public LootDeckBuilder ConfigLootDeck() => _lootBuilder;

    public MatchConfigBuilder InitialCoins(int amount) {
        throw new NotImplementedException();
    }

    public MatchConfigBuilder InitialLoot(int amount) {
        throw new NotImplementedException();
    }

    public MatchConfigBuilder StartingPlayer(int idx) {
        throw new NotImplementedException();
    }

    public MatchConfig Build() {
        throw new NotImplementedException();
    }
}

public class LootDeckBuilder(MatchConfigBuilder parent) {
    public MatchConfigBuilder Done() => parent;

    public LootDeckBuilder Add(string card, int amount) {
        throw new NotImplementedException();
    }
}
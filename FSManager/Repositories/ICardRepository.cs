namespace FSManager.Repositories;

public interface ICardRepository {
    public Task CreateCard(
        string key,
        string name,
        string type,
        int health,
        int attack,
        int evasion,
        string text,
        string script,
        int soulValue,
        string collectionKey,
        string defaultImageSrc
    );
    public Task<IEnumerable<CardModel>> AllCards();
    public Task<CardModel?> ByKey(string key);
    public Task<bool> RemoveCard(string key);
    public Task<IEnumerable<CardModel>> GetCards();
}
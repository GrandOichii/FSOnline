namespace FSManager.Repositories;

public interface ICardRepository {
    public Task<string> GetDefaultCardImageCollectionKey();
    public Task CreateCard(string key, string name, string text, string collectionKey, string defaultImageSrc);
    public Task<IEnumerable<CardModel>> AllCards();
    public Task<CardModel?> ByKey(string key);
    public Task<bool> RemoveCard(string key);
}
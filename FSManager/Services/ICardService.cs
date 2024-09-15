namespace FSManager.Services;

public interface ICardService {
    public Task<CardsPage> All(int page);
    public Task<CardsPage> Filter(CardFilter filter, int page);
    public Task<GetCard> Create(PostCard card);
    public Task<GetCard> Edit(string key, PostCard card);
    public Task Delete(string key);
    public Task<GetCardWithRelations> ByKey(string key);
    public Task<IEnumerable<string>> GetKeys();
    public Task<CardsPage> FromCollection(string collectionKey, int page);
    public Task<IEnumerable<GetCard>> OfType(string type);
    public Task<IEnumerable<GetCollection>> GetCollections();

    public Task<GetCardRelation> GetRelation(string key1, string key2);
    public Task CreateRelation(string cardKey, string relatedCardKey, CardRelationType relationType);
    public Task DeleteRelation(string cardKey, string relatedCardKey);
    public Task EditRelationType(string cardKey, string relatedCardKey, CardRelationType relationType);
}
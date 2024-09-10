namespace FSManager.Services;

public interface ICardService {
    public Task<IEnumerable<GetCard>> All(string? cardImageCollection = null);
    public Task<GetCard> Create(PostCard card);
    public Task Delete(string key);
    public Task<GetCardWithRelations> ByKey(string key);
    public Task<IEnumerable<string>> GetKeys();
    public Task<IEnumerable<GetCard>> FromCollection(string collectionKey);
    public Task<IEnumerable<GetCard>> OfType(string type);
    public Task<IEnumerable<GetCollection>> GetCollections();

    public Task CreateRelation(string cardKey, string relatedCardKey, CardRelationType relationType);
    public Task DeleteRelation(string cardKey, string relatedCardKey);
    public Task EditRelationType(string cardKey, string relatedCardKey, CardRelationType relationType);
}
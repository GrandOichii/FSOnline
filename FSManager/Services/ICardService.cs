namespace FSManager.Services;

public interface ICardService {
    public Task<IEnumerable<GetCard>> All(string? cardImageCollection = null);
    public Task<GetCard> Create(PostCard card);
    public Task Delete(string key);
    public Task<GetCard> ByKey(string key);
}
namespace FSManager.Services;

public interface ICardService {
    public Task<IEnumerable<GetCard>> All(string? cardImageCollection = null);
}
namespace FSManager.Repositories;

public interface ICollectionRepository {
    public Task<IEnumerable<CardCollection>> All();
}
namespace FSManager.Repositories;

public interface IMatchRepository {
    public Task<IEnumerable<MatchProcess>> All();
    public Task<MatchProcess> ById(string id);
    public Task Add(MatchProcess match);
}
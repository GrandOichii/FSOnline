
namespace FSManager.Repositories;

public class MatchRepository : IMatchRepository {
    public List<MatchProcess> Matches { get; } = [];

    public Task Add(MatchProcess match)
    {
        Matches.Add(match);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<MatchProcess>> All()
    {
        return Task.FromResult(Matches.AsEnumerable() );
    }

    public async Task<MatchProcess?> ById(string id) {
        return Matches.FirstOrDefault(m => m.ID.ToString() == id);
    }
}
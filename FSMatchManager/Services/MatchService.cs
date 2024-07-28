using Microsoft.Extensions.Options;

namespace FSMatchManager.Services;

public class MatchService(IOptions<MatchesSettings> settings) : IMatchService {
    private readonly IOptions<MatchesSettings> _settings = settings;
    public List<MatchProcess> Matches { get; } = [];

    public Task<List<MatchProcess>> All()
    {
        return Task.FromResult(Matches);
    }

    public Task<MatchProcess> Create(CreateMatch config)
    {
        // TODO validate
        // TODO
        var match = new MatchProcess();

        Matches.Add(match);
        
        return Task.FromResult(match);
    }
}
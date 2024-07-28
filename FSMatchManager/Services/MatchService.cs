using Microsoft.Extensions.Options;

namespace FSMatchManager.Services;

public class MatchService(IOptions<MatchesSettings> settings) : IMatchService {
    private readonly IOptions<MatchesSettings> _settings = settings;
    public List<MatchProcess> Matches { get; } = [];

    public Task<List<MatchProcess>> All()
    {
        return Task.FromResult(Matches);
    }

    public async Task<MatchProcess> WebSocketCreate(CreateMatchParams creationParams, WebSocketManager wsManager)
    {
        // TODO validate params
        var socket = await wsManager.AcceptWebSocketAsync();
        var match = new MatchProcess(creationParams);
        await match.AddWSPlayer(socket);

        Matches.Add(match);
        var _ = match.Configure;
        
        return match;
    }
}
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace FSMatchManager.Services;

public class MatchService(IOptions<MatchesSettings> settings) : IMatchService {
    private readonly IOptions<MatchesSettings> _settings = settings;
    public List<MatchProcess> Matches { get; } = [];

    public Task<List<MatchProcess>> All()
    {
        return Task.FromResult(Matches);
    }

    public async Task<MatchProcess> WebSocketCreate(WebSocketManager wsManager)
    {
        // TODO validate params
        var socket = await wsManager.AcceptWebSocketAsync();
        await socket.Write("Send match creation parameters");
        var paramsRaw = await socket.Read();
        var creationParams = JsonSerializer.Deserialize<CreateMatchParams>(paramsRaw);

        var match = new MatchProcess(creationParams);
        Matches.Add(match);
        var _ = match.Configure();

        await match.AddWSPlayer(socket);
        
        return match;
    }
}
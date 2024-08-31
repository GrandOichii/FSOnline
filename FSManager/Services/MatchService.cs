using System.Text.Json;
using Microsoft.Extensions.Options;

namespace FSManager.Services;

public class MatchService(IOptions<MatchesSettings> settings, ICardService cardService) : IMatchService {
    private readonly IOptions<MatchesSettings> _settings = settings;
    public List<MatchProcess> Matches { get; } = [];
    private readonly ICardService _cardService = cardService;

    public Task<List<MatchProcess>> All()
    {
        return Task.FromResult(Matches);
    }

    public Task<MatchProcess> Get(string matchId) {
        return Task.FromResult(
            Matches.FirstOrDefault(m => m.ID.ToString() == matchId)
                ?? throw new MatchNotFoundException($"Match with ID {matchId} not found")
        );
    }

    public async Task<MatchProcess> WebSocketCreate(WebSocketManager wsManager)
    {
        // TODO validate params
        var socket = await wsManager.AcceptWebSocketAsync();
        await socket.Write("mcp");
        var paramsRaw = await socket.Read();
        System.Console.WriteLine(paramsRaw);
        var creationParams = JsonSerializer.Deserialize<CreateMatchParams>(paramsRaw);

        var match = new MatchProcess(creationParams, _cardService);
        Matches.Add(match);
        var _ = match.Configure();

        await socket.Write($"id:{match.ID}");
        await match.AddWSPlayer(socket);
        
        return match;
    }

    public async Task WebSocketConnect(string matchId, WebSocketManager wsManager) {
        var match = await Get(matchId);
        if (!match.CanAddPlayer()) return;

        var socket = await wsManager.AcceptWebSocketAsync();

        await match.AddWSPlayer(socket);
    }
}
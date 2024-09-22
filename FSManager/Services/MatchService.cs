using System.Text.Json;
using Microsoft.Extensions.Options;

namespace FSManager.Services;

public class MatchService(IOptions<MatchesSettings> settings, ICardService cardService, IMatchRepository matchRepo, ILogger<MatchService> logger) : IMatchService {
    private readonly IOptions<MatchesSettings> _settings = settings;
    private readonly ICardService _cardService = cardService;
    private readonly IMatchRepository _matchRepo = matchRepo;
    private readonly ILogger<MatchService> _logger = logger;

    public async Task<IEnumerable<MatchProcess>> All()
    {
        return await _matchRepo.All();
    }

    public async Task<MatchProcess> Get(string matchId) {
        return await _matchRepo.ById(matchId)
            ?? throw new MatchNotFoundException($"Match with ID {matchId} not found")
        ;
    }

    public async Task<MatchProcess> WebSocketCreate(WebSocketManager wsManager)
    {
        // TODO validate params
        var socket = await wsManager.AcceptWebSocketAsync();
        await socket.Write("mcp");
        var paramsRaw = await socket.Read();
        var creationParams = JsonSerializer.Deserialize<CreateMatchParams>(paramsRaw);

        var match = new MatchProcess(creationParams, _cardService, _logger);
        await _matchRepo.Add(match);
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
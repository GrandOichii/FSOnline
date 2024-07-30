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

    public Task<MatchProcess> Get(string matchId) {
        return Task.FromResult(
            Matches.FirstOrDefault(m => m.ID.ToString() == matchId)
                ?? throw new MatchNotFoundException($"Match with ID {matchId} not found")
        );
    }

    public async Task<MatchProcess> WebSocketCreate(WebSocketManager wsManager)
    {
        // TODO validate params
        System.Console.WriteLine("1");
        var socket = await wsManager.AcceptWebSocketAsync();
        System.Console.WriteLine("2");
        await socket.Write("Send match creation parameters");
        System.Console.WriteLine("3");
        var paramsRaw = await socket.Read();
        System.Console.WriteLine("4");
        System.Console.WriteLine(paramsRaw);
        var creationParams = JsonSerializer.Deserialize<CreateMatchParams>(paramsRaw);
        System.Console.WriteLine("5");

        var match = new MatchProcess(creationParams);
        Matches.Add(match);
        var _ = match.Configure();

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
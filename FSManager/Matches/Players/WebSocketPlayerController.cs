using System.Net.WebSockets;
using System.Text;

namespace FSManager.Matches.Players;

public class WebSocketIOHandler(WebSocket socket) : IIOHandler
{
    private readonly WebSocket _socket = socket;

    public async Task Close()
    {
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "MatchEnd", CancellationToken.None);
    }

    public async Task<string> Read()
    {
        return await _socket.Read();
    }

    public async Task Write(string msg)
    {
        await _socket.Write(msg);
    }
}
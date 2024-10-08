using System.Net.Sockets;
using FSCore.Utility;

namespace FSManager.Matches.Players;

/// <summary>
/// Checker for a TCP connection
/// </summary>
public class TcpConnectionChecker(TcpClient socket) : IConnectionChecker {
    /// <summary>
    /// TCP socket
    /// </summary>
    private readonly TcpClient _socket = socket;

    public async Task<bool> Check()
    {
        try {
            NetUtility.Write(_socket.GetStream(), "ping");
            // TODO change to 5000
            _socket.ReceiveTimeout = 50000;
            var resp = await Read();
            _socket.ReceiveTimeout = 0;
            return resp == "pong";
        } catch {
            return false;
        }
    }

    public Task<string> Read()
    {
        return Task.FromResult(
            NetUtility.Read(_socket.GetStream())
        );
    }

    public Task Write(string msg)
    {
        NetUtility.Write(_socket.GetStream(), msg);
        return Task.CompletedTask;
    }
}


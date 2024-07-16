using System.Net.Sockets;

namespace FSCore.Matches.Players.Controllers;

public class TcpIOHandler : IIOHandler
{
    private readonly TcpClient _client;

    public TcpIOHandler(TcpClient client) {
        _client = client;
    }

    public Task Close()
    {
        _client.Close();
        return Task.CompletedTask;
    }

    public Task<string> Read()
    {
        return Task.FromResult(
            NetUtility.Read(_client.GetStream())
        );
    }

    public Task Write(string msg)
    {
        NetUtility.Write(_client.GetStream(), msg);
        return Task.CompletedTask;
    }
}

using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace CWClient.v1.Connection;


public class TcpConnection : IConnection
{
	private readonly TcpClient _client;

	#nullable enable
	private event IConnection.MessageHandler? OnReceive;
	private event IConnection.CloseHandler? OnClose;
	#nullable disable

	public TcpConnection(TcpClient client) {
		_client = client;
	}

	public void StartReceiveLoop() {
		Task.Run(async () => {
			// _client.ReceiveTimeout = 50;
			while (_client.Connected) {
				try {
					var message = await Read();
					await OnReceive?.Invoke(message);
				} catch {
					continue;
				}
			}
			OnClose?.Invoke();
		});
	}

	public void SubscribeToUpdate(IConnection.MessageHandler func)
	{
		OnReceive += func;
	}

	public void SubscribeToClose(IConnection.CloseHandler func)
	{
		OnClose += func;
	}

	public Task Write(string message)
	{
		NetUtility.Write(_client.GetStream(), message);
		return Task.CompletedTask;
	}

	public Task Close()
	{
		_client.Close();
		return Task.CompletedTask;
	}

	public Task<string> Read() {
		var result = NetUtility.Read(_client.GetStream());
		return Task.FromResult(result);
	}
}

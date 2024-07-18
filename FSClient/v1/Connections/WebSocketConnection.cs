using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace CWClient.v1.Connection;


public class WebSocketConnection : IConnection
{
	private readonly ClientWebSocket _client;

	#nullable enable
	private event IConnection.MessageHandler? OnReceive;

	private event IConnection.CloseHandler? OnClose;
	#nullable disable

	public WebSocketConnection(ClientWebSocket client)
	{
		_client = client;
	}

	public void StartReceiveLoop() {
		Task.Run(async () =>
		{
			while (_client.State == WebSocketState.Open)
			{
				var message = await Read();
				await OnReceive?.Invoke(message);
			}
			OnClose?.Invoke();
		});
	}

	public async Task<string> Read() {
		WebSocketReceiveResult result;
		var buffer = new ArraySegment<byte>(new byte[1024]);
		var message = new StringBuilder();
		do
		{
			result = await _client.ReceiveAsync(buffer, CancellationToken.None);
			string messagePart = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
			message.Append(messagePart);
		}
		while (!result.EndOfMessage);
		return message.ToString();
	}

	public void SubscribeToUpdate(IConnection.MessageHandler func)
	{
		OnReceive += func;
	}

	public async Task Write(string message)
	{
		await _client.SendAsync(message.ToUtf8Buffer(), WebSocketMessageType.Text, true, CancellationToken.None);
	}

	public void SubscribeToClose(IConnection.CloseHandler func)
	{
		OnClose += func;
	}

	public Task Close()
	{
		try {
			// TODO? not awaiting the call fixes the window not closing until the socket is closed
			_ = _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "ClientClose", CancellationToken.None);
		} catch {}
		
		return Task.CompletedTask;
	}
}

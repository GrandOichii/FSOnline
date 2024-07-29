using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace CWClient.v1.Connection.Wrappers;

public partial class WebSocketConnectionWrapper : ConnectionWrapper
{
	#region Signals

	[Signal]
	public delegate void ConnectedEventHandler();
	[Signal]
	public delegate void MessageReceivedEventHandler(string message);

	#endregion

	private WebSocketConnection _connection;

	public void Create(string address) {
		// TODO
		var client = new ClientWebSocket();
		GD.Print($"ws://{address}/Create");
		client.ConnectAsync(new Uri($"ws://{address}/Create"), CancellationToken.None).Wait();
		GD.Print("Connected!");
		_connection = new WebSocketConnection(client);        

		_connection.SubscribeToUpdate(OnRead);
		EmitSignal(SignalName.Connected);
		_connection.StartReceiveLoop();
	}

	public void Connect(string url, string matchId) {
		// var client = new ClientWebSocket();

		// await client.ConnectAsync(new Uri(url), CancellationToken.None);
		
		// var client = new TcpClient();
		// var address = url + ":" + port;
		// client.Connect(IPEndPoint.Parse(address));
		// _connection = new WebSocketConnection(client);
		// _connection.SubscribeToUpdate(OnRead);
		// EmitSignal(SignalName.Connected);
		// _connection.StartReceiveLoop();
	}

	public async Task WriteAsync(string msg) {
		await _connection.Write(msg);
	}

	public void Write(string msg) {
		GD.Print("writing " + msg);
		WriteAsync(msg)
			.Wait();
	}

	private Task OnRead(string msg) {
		GD.Print("Read " + msg);
		CallDeferred("emit_signal", SignalName.MessageReceived, msg);
		return Task.CompletedTask;
	}

}

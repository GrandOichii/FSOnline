using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CWClient.v1.Connection.Wrappers;

public partial class TcpConnectionWrapper : ConnectionWrapper
{
	#region Signals

	[Signal]
	public delegate void ConnectedEventHandler();
	[Signal]
	public delegate void MessageReceivedEventHandler(string message);

	#endregion

	private TcpConnection _connection;

	public void Connect(string url, int port) {
		var client = new TcpClient();
		var address = url + ":" + port;
		client.Connect(IPEndPoint.Parse(address));
		_connection = new TcpConnection(client);
		_connection.SubscribeToUpdate(OnRead);
		EmitSignal(SignalName.Connected);
		_connection.StartReceiveLoop();
	}

	public async Task WriteAsync(string msg) {
		await _connection.Write(msg);
	}

	public void Write(string msg) {
		WriteAsync(msg)
			.Wait();
	}

	private Task OnRead(string msg) {
		CallDeferred("emit_signal", SignalName.MessageReceived, msg);
		return Task.CompletedTask;
	}

}

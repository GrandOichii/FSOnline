using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace CWClient.v1.Connection;

public interface IConnection {

	public delegate Task MessageHandler(string message);
	public void StartReceiveLoop();
	public delegate void CloseHandler();
	public Task Write(string message);
	public void SubscribeToUpdate(MessageHandler func);
	public void SubscribeToClose(CloseHandler func);
	public Task Close();
	public Task<string> Read();

	// public async Task SendData(string name, string deck, string password) {
	// 	var pData = new PlayerData {
	// 		Name = name,
	// 		Deck = deck,
	// 		Password = password
	// 	};
	// 	var data = JsonSerializer.Serialize(pData, Common.JSON_SERIALIZATION_OPTIONS);
	// 	await Write(data);
	// }
}
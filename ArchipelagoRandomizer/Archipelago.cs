using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System;

namespace DDoor.ArchipelagoRandomizer;

class Archipelago
{
	public static event Action OnConnected;
	public static event Action OnDisconnected;
	private static readonly Archipelago instance = new();

	public static Archipelago Instance => instance;
	public ArchipelagoSession Session { get; private set; }
	private bool IsConnected => Session != null && Session.Socket.Connected;

	private Archipelago() { }

	public LoginSuccessful Connect(APConnectionInfo info)
	{
		Session = ArchipelagoSessionFactory.CreateSession(info.URL, info.Port);

		LoginResult loginResult = Session.TryConnectAndLogin(
			"Death's Door",
			info.SlotName,
			ItemsHandlingFlags.AllItems,
			password: info.Password,
			requestSlotData: false
		);

		switch (loginResult)
		{
			case LoginFailure failure:
				string errors = string.Join(", ", failure.Errors);
				Logger.LogError($"Failed to connect to Archipelago: {errors}");
				throw new LoginValidationException(errors);
			case LoginSuccessful success:
				Session.Socket.SocketClosed += OnSocketClosed;
				OnConnected?.Invoke();
				Logger.Log($"Successfully connected to Archipelago at {info.URL}:{info.Port} as {info.SlotName} on team {success.Team}. Have fun!");
				return success;
			default:
				throw new LoginValidationException($"Unexpected LoginResult type when connecting to Archipelago: {loginResult}");
		}
	}

	public void Disconnect()
	{
		if (Session?.Socket != null)
		{
			Session.Socket.SocketClosed -= OnSocketClosed;

			if (Session.Socket.Connected)
			{
				Session.Socket.DisconnectAsync();
			}

			Session = null;
		}
	}

	private void OnSocketClosed(string reason)
	{
		OnDisconnected?.Invoke();
	}

	public struct APConnectionInfo
	{
		public string URL { get; set; }
		public int Port { get; set; }
		public string SlotName { get; set; }
		public string Password { get; set; }
	}
}
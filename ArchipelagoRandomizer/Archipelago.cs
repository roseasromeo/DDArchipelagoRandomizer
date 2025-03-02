using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DDoor.ArchipelagoRandomizer;

internal class Archipelago
{
	public static event Action OnConnected;
	public static event Action OnDisconnected;
	private static readonly Archipelago instance = new();
	private Dictionary<string, object> slotData;

	public static Archipelago Instance => instance;
	public ArchipelagoSession Session { get; private set; }
	public bool IsConnected => Session != null && Session.Socket.Connected;
	public PlayerInfo CurrentPlayer
	{
		get
		{
			if (Session == null)
			{
				return null;
			}

			return Session.Players.GetPlayerInfo(Session.ConnectionInfo.Slot);
		}
	}

	private Archipelago() { }

	public LoginSuccessful Connect(APConnectionInfo info, int saveInfoToSlotIndex = 0)
	{
		Session = ArchipelagoSessionFactory.CreateSession(info.URL, info.Port);

		LoginResult loginResult = Session.TryConnectAndLogin(
			"Death's Door",
			info.SlotName,
			ItemsHandlingFlags.AllItems,
			password: info.Password,
			requestSlotData: true
		);

		switch (loginResult)
		{
			case LoginFailure failure:
				string errors = string.Join(", ", failure.Errors);
				throw new LoginValidationException($"Failed to connect to Archipelago: {errors}");
			case LoginSuccessful success:
				Session.Socket.SocketClosed += OnSocketClosed;
				slotData = success.SlotData;
				OnConnected?.Invoke();
				SaveConnectionInfo(info, saveInfoToSlotIndex);
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
			Logger.Log("Disconnected from Archipelago server.");
		}
	}

	public T GetSlotData<T>(string key)
	{
		object value = default(T);

		if (slotData == null || !slotData.TryGetValue(key, out value))
		{
			Logger.LogError($"No slot data with key '{key}' was found, returning null.");
			return (T)value;
		}

		return (T)value;
	}

	private void OnSocketClosed(string reason)
	{
		OnDisconnected?.Invoke();
	}

	private readonly string apConnectionInfoSavePath = $"{Application.persistentDataPath}/SAVEDATA/Save_slot#_APConnectionInfo.json";

	private void SaveConnectionInfo(APConnectionInfo info, int saveInfoToSlotIndex)
	{
		string path = apConnectionInfoSavePath.Replace("#", (saveInfoToSlotIndex + 1).ToString());
		string json = JsonConvert.SerializeObject(info, Formatting.Indented);
		File.WriteAllText(path, json);
		Logger.Log($"Saved AP connection data to: {path}");
	}

	public APConnectionInfo GetConnectionInfoForFile(int slotIndex)
	{
		string path = apConnectionInfoSavePath.Replace("#", (slotIndex + 1).ToString());
		string json = File.ReadAllText(path);
		APConnectionInfo info = JsonConvert.DeserializeObject<APConnectionInfo>(json);

		if (info == null)
		{
			Logger.LogError($"Failed to find AP connection info save file for slot {slotIndex}:");
		}

		return info;
	}

	public class APConnectionInfo
	{
		public string URL { get; set; }
		public int Port { get; set; }
		public string SlotName { get; set; }
		public string Password { get; set; }
	}
}
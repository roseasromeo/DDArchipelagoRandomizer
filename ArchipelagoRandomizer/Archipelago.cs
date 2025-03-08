using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DDoor.ArchipelagoRandomizer;

internal class Archipelago
{
	public static event Action OnConnected;
	public static event Action OnDisconnected;
	private static readonly Archipelago instance = new();
	private readonly string apConnectionInfoSavePath = $"{Application.persistentDataPath}/SAVEDATA/Save_slot#_APConnectionInfo.json";
	private Dictionary<string, object> slotData;
	private IEnumerator checkItemsReceived;
	private IEnumerator incomingItemHandler;
	private IEnumerator outgoingItemHandler;
	private ConcurrentQueue<(ItemInfo item, int index)> incomingItems;
	private ConcurrentQueue<ItemInfo> outgoingItems;
	private int itemIndex;
	private bool isConnected;
	private bool hasCompleted;
	private readonly float itemReceiveDelay = 3f;

	public static Archipelago Instance => instance;
	public ArchipelagoSession Session { get; private set; }
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
	public List<ItemRandomizer.ItemPlacement> ScoutedPlacements { get; private set; }

	private Archipelago() { }

	public async Task<LoginSuccessful> Connect(APConnectionInfo info, int saveInfoToSlotIndex = 0)
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
				await OnSocketOpened(success, info, saveInfoToSlotIndex);
				Logger.Log($"Successfully connected to Archipelago at {info.URL}:{info.Port} as {info.SlotName} on team {success.Team}. Have fun!");
				return success;
			default:
				throw new LoginValidationException($"Unexpected LoginResult type when connecting to Archipelago: {loginResult}");
		}
	}

	public void Disconnect()
	{
		if (!isConnected)
		{
			return;
		}

		Session.Socket.SocketClosed -= OnSocketClosed;
		Session.Socket.DisconnectAsync();
		Session = null;
	}

	public void Update()
	{
		if (!isConnected)
		{
			return;
		}

		checkItemsReceived?.MoveNext();

		if (CanPlayerReceiveItems())
		{
			incomingItemHandler?.MoveNext();
			outgoingItemHandler?.MoveNext();
		}
	}

	public void SendLocationChecked(string locationName)
	{
		if (!isConnected)
		{
			return;
		}

		long locationId = Session.Locations.GetLocationIdFromName(Session.ConnectionInfo.Game, locationName);
		Session.Locations.CompleteLocationChecks(locationId);
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

	private async Task OnSocketOpened(LoginSuccessful loginSuccess, APConnectionInfo connectionInfo, int saveInfoToSlotIndex)
	{
		slotData = loginSuccess.SlotData;
		itemIndex = TitleScreen.instance.saveMenu.saveSlots[TitleScreen.instance.index].saveFile.GetCountKey("AP_ItemsReceived");
		checkItemsReceived = CheckItemsReceieved();
		incomingItemHandler = IncomingItemHandler();
		outgoingItemHandler = OutgoingItemHandler();
		incomingItems = new ConcurrentQueue<(ItemInfo item, int index)>();
		outgoingItems = new ConcurrentQueue<ItemInfo>();
		SaveConnectionInfo(connectionInfo, saveInfoToSlotIndex);
		Session.Socket.SocketClosed += OnSocketClosed;

		await ScoutAllLocations();

		isConnected = true;
		OnConnected?.Invoke();
	}

	private void OnSocketClosed(string reason)
	{
		incomingItemHandler = null;
		outgoingItemHandler = null;
		incomingItems = new ConcurrentQueue<(ItemInfo item, int index)>();
		outgoingItems = new ConcurrentQueue<ItemInfo>();
		isConnected = false;
		OnDisconnected?.Invoke();
		Logger.Log("Disconnected from Archipelago!");
	}

	private IEnumerator CheckItemsReceieved()
	{
		while (isConnected)
		{
			if (Session.Items.Index > itemIndex)
			{
				ItemInfo item = Session.Items.AllItemsReceived[itemIndex];
				incomingItems.Enqueue((item, itemIndex));
				itemIndex++;
				yield return true;
			}
			else
			{
				yield return true;
				continue;
			}
		}
	}

	private IEnumerator IncomingItemHandler()
	{
		while (isConnected)
		{
			if (!incomingItems.TryPeek(out (ItemInfo item, int index) pendingItem))
			{
				yield return true;
				continue;
			}

			// Add delay between each item received so player has time to read the notifications
			float delay = Time.time;
			while (Time.time - delay < (incomingItems.Count < 5 ? itemReceiveDelay : 1))
			{
				yield return null;
			}

			ItemInfo item = pendingItem.item;
			ItemRandomizer.Instance.ReceievedItem(item.ItemDisplayName, item.LocationDisplayName, item.Player);
			incomingItems.TryDequeue(out _);

			yield return true;
		}
	}

	private IEnumerator OutgoingItemHandler()
	{
		while (isConnected)
		{
			if (!outgoingItems.TryDequeue(out ItemInfo item))
			{
				yield return true;
				continue;
			}

			if (item.Player != CurrentPlayer)
			{
				Logger.Log($"Sent {item.ItemName} at {item.LocationDisplayName} to {item.Player.Name}");
			}

			yield return true;
		}
	}

	private void SaveConnectionInfo(APConnectionInfo info, int saveInfoToSlotIndex)
	{
		string path = apConnectionInfoSavePath.Replace("#", (saveInfoToSlotIndex + 1).ToString());
		string json = JsonConvert.SerializeObject(info, Formatting.Indented);
		File.WriteAllText(path, json);
		Logger.Log($"Saved AP connection data to: {path}");
	}

	private async Task ScoutAllLocations()
	{
		ScoutedPlacements = (await Session.Locations.ScoutLocationsAsync(
			Session.Locations.AllLocations.ToArray()
		)).Select(kvp => new ItemRandomizer.ItemPlacement(
			kvp.Value.ItemDisplayName,
			kvp.Value.LocationName,
			kvp.Value.Player.Name,
			kvp.Value.Player != CurrentPlayer
		)).ToList();
	}

	private bool CanPlayerReceiveItems()
	{
		return (
			!hasCompleted &&
			PlayerGlobal.instance != null &&
			!PlayerGlobal.instance.InputPaused() &&
			PlayerGlobal.instance.IsAlive()
		);
	}

	public class APConnectionInfo
	{
		public string URL { get; set; }
		public int Port { get; set; }
		public string SlotName { get; set; }
		public string Password { get; set; }
	}
}
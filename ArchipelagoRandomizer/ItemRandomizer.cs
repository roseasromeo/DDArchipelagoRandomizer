using HarmonyLib;
using UnityEngine;
using IC = DDoor.ItemChanger;

namespace DDoor.ArchipelagoRandomizer;

internal class ItemRandomizer : MonoBehaviour
{
	private static ItemRandomizer instance;
	private IC.SaveData icSaveData;

	public static ItemRandomizer Instance => instance;

	private void Awake()
	{
		instance = this;
		IC.ItemIcons.AddPath(System.IO.Path.GetDirectoryName(typeof(Plugin).Assembly.Location) + "/Resources/Item Changer Icons");
	}

	private void OnEnable()
	{
		PlaceItems();
		Logger.Log("Item randomizer started!");
	}

	private void Update()
	{
		Archipelago.Instance.Update();
	}

	public void ReceievedItem(string itemName, string location, int playerSlot)
	{
		if (GameSave.currentSave.IsKeyUnlocked($"AP_PickedUp-{location}"))
		{
			Logger.Log($"Already received item at {location} when it was picked up, so don't receive it again");
			return;
		}

		string playerName = Archipelago.Instance.Session.Players.GetPlayerName(playerSlot);
		bool receivedFromSelf = playerSlot == Archipelago.Instance.CurrentPlayer.Slot;
		Sprite icon;
		string message;

		if (!IC.Predefined.TryGetItem(itemName, out IC.Item item))
		{
			Logger.LogError($"Received unknown item {itemName} from {playerName}");
			icon = IC.ItemIcons.Get("Unknown");
			message = $"You got an unknown item: {itemName}. Please report!";
			IC.CornerPopup.Show(icon, message);
			return;
		}

		Logger.Log($"Received {itemName} from {playerName}");
		icon = IC.ItemIcons.Get(item.Icon);
		message = $"You got {item.DisplayName}";

		if (!receivedFromSelf)
		{
			message += $" from {playerName}!";
		}

		GameSave.currentSave.IncreaseCountKey("AP_ItemsReceived");
		IC.CornerPopup.Show(icon, message);
		item?.Trigger();

		// Update recent items display
		IC.TrackerLogEntry logEntry = new IC.TrackerLogEntry()
		{
			LocationName = location,
			ItemName = itemName,
			ItemDisplayName = receivedFromSelf ? item.DisplayName : item.DisplayName + $" from {playerName}",
			ItemIcon = item.Icon
		};
		icSaveData.AddToTrackerLog(logEntry);

		GameSave.SaveGameState();
	}

	private void PickedUpItem(DDItem item)
	{
		GameSave.currentSave.SetKeyState($"AP_PickedUp-{item.Location}", true, true);
		Archipelago.Instance.GetAPSaveData().AddCheckedLocation(item.Location);
		Archipelago.Instance.SendLocationChecked(item.Location);
	}

	private void PlaceItems()
	{
		icSaveData = IC.SaveData.Open();

		// Get ItemPlacements from scouted placements
		foreach (ItemPlacement itemPlacement in Archipelago.Instance.ScoutedPlacements)
		{
			// Get predefined item to use its icon
			IC.Predefined.TryGetItem(itemPlacement.Item, out IC.Item predefinedItem);

			// Don't use predefined icon for weapons because it errors at this point, since player doesn't exist yet,
			// so it doesn't have a way to reference WeaponSwitcher to determine upgrade level
			string icon = itemPlacement.IsForAnotherPlayer
				? "AP"
				: (itemPlacement.Item is "Bomb" or "Fire" or "Hookshot")
				? itemPlacement.Item
				: predefinedItem?.Icon ?? "Unknown";

			// Change name of item so it displays receiving player's name in message
			string itemName = itemPlacement.IsForAnotherPlayer
				? $"{itemPlacement.Item} for {itemPlacement.ForPlayer}"
				: itemPlacement.Item;

			// Place item. Using custom item here so we can override the Trigger() method
			// for knowing when an item was picked up to send it to server
			IC.Item item = new DDItem(itemName, icon, itemPlacement.Location);
			icSaveData.Place(item, itemPlacement.Location);

			Logger.Log($"Placed {itemPlacement.Item} for {itemPlacement.ForPlayer} at {itemPlacement.Location}");
		}

		// Determine starting weapon
		string startingWeaponName = Archipelago.Instance.GetSlotData<string>("start_weapon");
		string startingWeaponId = startingWeaponName switch
		{
			"Rogue Daggers" => "daggers",
			"Discarded Umbrella" => "umbrella",
			"Reaper's Greatsword" => "sword_heavy",
			"Thunder Hammer" => "hammer",
			_ => "sword"
		};
		icSaveData.StartingWeapon = startingWeaponId;

		// Save, since ItemChanger doesn't do it for us due to when we run this method
		GameSave saveFile = TitleScreen.instance.saveMenu.saveSlots[TitleScreen.instance.saveMenu.index].saveFile;
		saveFile.weaponId = icSaveData.StartingWeapon;
		saveFile.Save();
	}

	public struct ItemPlacement(string item, string location, string forPlayer, bool isForAnotherPlayer)
	{
		public string Item { get; private set; } = item;
		public string Location { get; private set; } = location;
		public string ForPlayer { get; private set; } = forPlayer;
		public bool IsForAnotherPlayer { get; private set; } = isForAnotherPlayer;
	}

	private readonly struct DDItem : IC.Item
	{
		public string DisplayName { get; }
		public string Icon { get; }
		public string Location { get; }

		public void Trigger()
		{
			Instance.PickedUpItem(this);
		}

		public DDItem(string displayName, string icon, string location)
		{
			DisplayName = displayName;
			Icon = icon;
			Location = location;
		}
	}

	[HarmonyPatch]
	private class Patches
	{
		/// <summary>
		/// Sends completion for main ending
		/// </summary>
		[HarmonyPrefix, HarmonyPatch(typeof(SoulAbsorbCutscene), nameof(SoulAbsorbCutscene.StartCutscene))]
		private static void EndGameCsPatch()
		{
			Archipelago.Instance.SendCompletion();
		}
	}
}
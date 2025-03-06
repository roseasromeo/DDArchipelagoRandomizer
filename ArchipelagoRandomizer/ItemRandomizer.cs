using UnityEngine;
using IC = DDoor.ItemChanger;

namespace DDoor.ArchipelagoRandomizer;

internal class ItemRandomizer : MonoBehaviour
{
	private static ItemRandomizer instance;

	public static ItemRandomizer Instance => instance;

	private void Awake()
	{
		instance = this;
		IC.ItemIcons.AddPath(System.IO.Path.GetDirectoryName(typeof(Plugin).Assembly.Location) + "/Icons");
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

	public void ReceievedItem(string itemName, int playerSlot)
	{
		if (GameSave.currentSave.IsKeyUnlocked($"PickedUp{itemName}"))
		{
			Logger.Log($"Already picked up {itemName}, so don't receive it again");
			return;
		}

		string playerName = Archipelago.Instance.Session.Players.GetPlayerName(playerSlot);
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
		else
		{
			Logger.Log($"Received {itemName} from {playerName}");
			icon = IC.ItemIcons.Get(item.Icon);
			message = $"You got {item.DisplayName} from {playerName}!";
		}

		IC.CornerPopup.Show(icon, message);
		item?.Trigger();
	}

	private void PickedUpItem(DDItem item)
	{
		Logger.Log($"Picked up {item.DisplayName} at {item.Location}!");
		GameSave.currentSave.SetKeyState($"PickedUp{item.DisplayName}", true, true);
		Archipelago.Instance.SendLocationChecked(item.Location);
	}

	private void PlaceItems()
	{
		// Get ItemPlacements from scouted placements
		IC.SaveData data = IC.SaveData.Open();

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
			data.Place(item, itemPlacement.Location);

			Logger.Log($"Placed {itemPlacement.Item} for {itemPlacement.ForPlayer} at {itemPlacement.Location}");
		}

		// Save, since ItemChanger doesn't do it for us due to when we run this method
		TitleScreen.instance.saveMenu.saveSlots[TitleScreen.instance.index].saveFile.Save();
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
}
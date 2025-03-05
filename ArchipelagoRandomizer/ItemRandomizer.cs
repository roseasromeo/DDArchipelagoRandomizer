using System.Collections.Generic;
using System.Linq;
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

	public void ReceievedItem(string itemName, string playerName)
	{
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

	private void PlaceItems()
	{
		List<ItemPlacement> itemPlacements = Archipelago.Instance.ScoutedPlacements.Select(kvp => new ItemPlacement(kvp.Value, kvp.Key)).ToList();
		IC.SaveData data = IC.SaveData.Open();

		foreach (ItemPlacement itemPlacement in itemPlacements)
		{
			data.Place(itemPlacement.Item, itemPlacement.Location);
			Logger.Log($"Placed {itemPlacement.Item} at {itemPlacement.Location}");
		}

		TitleScreen.instance.saveMenu.saveSlots[TitleScreen.instance.index].saveFile.Save();
	}

	private struct ItemPlacement(string item, string location)
	{
		public string Item { get; private set; } = item;
		public string Location { get; private set; } = location;
	}
}
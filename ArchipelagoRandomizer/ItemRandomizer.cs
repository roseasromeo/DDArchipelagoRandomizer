using System.Collections.Generic;
using UnityEngine;
using IC = DDoor.ItemChanger;

namespace DDoor.ArchipelagoRandomizer;

class ItemRandomizer : MonoBehaviour
{
	private static ItemRandomizer instance;

	public static ItemRandomizer Instance => instance;

	private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		// Test items
		// TOOD: Replace with actual item placements received from AP
		List<ItemPlacement> itemPlacements =
		[
			new("Hookshot", "Seed-Cemetery Left of Main Entrance"),
			new("Bomb", "Soul Orb-Cemetery Under Bridge"),
			new("Fire", "Seed-Cemetery Broken Bridge"),
			new("Arrow Upgrade", "Seed-Cemetery Near Tablet Gate"),
		];

		PlaceItems(itemPlacements);
		Logger.Log("Item randomizer started!");
	}

	private void PlaceItems(List<ItemPlacement> itemPlacements)
	{
		IC.SaveData data = IC.SaveData.Open();

		foreach (ItemPlacement itemPlacement in itemPlacements)
		{
			data.Place(itemPlacement.Item, itemPlacement.Location);
			Logger.Log($"Placed {itemPlacement.Item} at {itemPlacement.Location}");
		}
	}

	private struct ItemPlacement(string item, string location)
	{
		public string Item { get; private set; } = item;
		public string Location { get; private set; } = location;
	}
}
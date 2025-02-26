using IC = DDoor.ItemChanger;

namespace DDoor.ArchipelagoRandomizer;

class ItemRandomizer
{
	public ItemRandomizer()
	{
		OnFileStarted();
	}

	private void OnFileStarted()
	{
		var data = IC.SaveData.Open();
		data.Place("Hookshot", "Seed-Cemetery Left of Main Entrance");
		data.Place("Fire", "Seed-Cemetery Broken Bridge");
		data.Place("Arrow Upgrade", "Seed-Cemetery Near Tablet Gate");
		data.Place("Bomb", "Soul Orb-Cemetery Under Bridge");
		Logger.Log("Item randomizer started!");
	}
}
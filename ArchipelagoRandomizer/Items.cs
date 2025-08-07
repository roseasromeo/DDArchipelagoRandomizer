using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace DDoor.ArchipelagoRandomizer;

#nullable enable
public static class Items
{
    static Items()
    {
        PopulateItemsFromJson();
    }
    public readonly struct Item(
            string itemChangerName,
            long apItemId
        )
    {
        public readonly string itemChangerName = itemChangerName;
        public readonly long apItemId = apItemId;
    }

    public static List<Item> itemData = [];

    private static void PopulateItemsFromJson()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("DDoor.ArchipelagoRandomizer.Data.Items.json"));
        List<Item>? tempItemData = JsonConvert.DeserializeObject<List<Item>>(reader.ReadToEnd());
        if (tempItemData != null)
        {
            itemData = tempItemData;
        }
        else
        {
            throw new InvalidDataException("Bundled Items.json was not readable. Please report this error to the developers");
        }
    }
}
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace DDoor.ArchipelagoRandomizer;

public static class Locations
{
    static Locations()
    {
        PopulateLocationsFromJson();
    }
    public readonly struct Location(
        string itemChangerName,
        long apLocationId
    )
    {
        public readonly string itemChangerName = itemChangerName;
        public readonly long apLocationId = apLocationId;
    }
    public static List<Location> locationData;

    private static void PopulateLocationsFromJson()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
		using StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("DDoor.ArchipelagoRandomizer.Data.Locations.json"));
		locationData = JsonConvert.DeserializeObject<List<Location>>(reader.ReadToEnd());
	}
}

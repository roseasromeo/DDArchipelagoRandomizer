using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DDoor.ArchipelagoRandomizer;


public static class SceneTransitions
{
    static SceneTransitions()
    {
        PopulateTransitionFromJson();
    }
    public readonly struct SceneTransition(
            string apName,
            string loadingZoneId,
            string sceneName
        )
    {
        public readonly string apName = apName;
        public readonly string loadingZoneId = loadingZoneId;
        public readonly string sceneName = sceneName;
    }

    private static List<SceneTransition> transitionData;

    private static void PopulateTransitionFromJson()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("DDoor.ArchipelagoRandomizer.Data.SceneTransitions.json"));
        transitionData = JsonConvert.DeserializeObject<List<SceneTransition>>(reader.ReadToEnd());
    }

    internal static SceneTransition? GetConnectedSceneTransition(string loadingZoneId, string sceneName)
    {
        // Plugin.Logger.LogDebug(loadingZoneId);
        // Plugin.Logger.LogDebug(sceneName);
        // Plugin.Logger.LogDebug(transitionData.First(scene => scene.loadingZoneId.Equals(loadingZoneId, System.StringComparison.OrdinalIgnoreCase)).apName);
        // Plugin.Logger.LogDebug(transitionData.First(scene => scene.sceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase)).apName);
        string entranceName = transitionData.First(scene => scene.loadingZoneId.Equals(loadingZoneId, System.StringComparison.OrdinalIgnoreCase) && scene.sceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase)).apName;
        // Plugin.Logger.LogDebug(entranceName);
		if (EntranceRandomizer.Instance.GetEntrancePairings().TryGetValue(entranceName, out string exitName))
		{
			Plugin.Logger.LogDebug(exitName);
			return transitionData.First(scene => scene.apName == exitName);
		}
		else
		{
			return null;
		}

	}

}
#nullable disable
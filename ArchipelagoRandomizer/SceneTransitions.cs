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
            string toSceneName,
            string originSceneName
        )
    {
        public readonly string apName = apName;
        public readonly string loadingZoneId = loadingZoneId;
        public readonly string toSceneName = toSceneName;
        public readonly string originSceneName = originSceneName;
    }

    private static List<SceneTransition> transitionData;

    private static void PopulateTransitionFromJson()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using StreamReader reader = new StreamReader(assembly.GetManifestResourceStream("DDoor.ArchipelagoRandomizer.Data.SceneTransitions.json"));
        transitionData = JsonConvert.DeserializeObject<List<SceneTransition>>(reader.ReadToEnd());
    }

    internal static SceneTransition? GetThisSceneTransition(string loadingZoneId, string sceneName) => transitionData.First(scene => scene.loadingZoneId.Equals(loadingZoneId.Replace("avarice_", ""), System.StringComparison.OrdinalIgnoreCase) && scene.toSceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase));


    internal static SceneTransition? GetConnectedSceneTransition(string loadingZoneId, string sceneName)
    {
        string entranceName = GetThisSceneTransition(loadingZoneId, sceneName)?.apName;
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
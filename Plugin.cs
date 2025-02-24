using BepInEx;
using BepInEx.Logging;
using AGM = DDoor.AlternativeGameModes;

namespace ArchipelagoRandomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
	internal static new ManualLogSource Logger;

	private void Awake()
	{
		// Plugin startup logic
		Logger = base.Logger;
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		AGM.AlternativeGameModes.Add("Archipelago Randomizer", () =>
		{
			Logger.LogInfo("test!");
		});
	}
}
using BepInEx;
using BepInEx.Logging;
using AGM = DDoor.AlternativeGameModes;

namespace DDoor.ArchipelagoRandomizer;

[BepInPlugin("deathsdoor.archipelagorandomizer", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("deathsdoor.itemchanger"), BepInDependency("deathsdoor.alternativegamemodes")]
public class Plugin : BaseUnityPlugin
{
	internal static new ManualLogSource Logger;

	public int InitStatus { get; internal set; } = 0;

	private void Awake()
	{
		try
		{
			// Plugin startup logic
			Logger = base.Logger;
			Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

			AGM.AlternativeGameModes.Add("Archipelago", () =>
			{
				new ItemRandomizer();
			});

			InitStatus = 1;
		}
		catch (System.Exception err)
		{
			InitStatus = 2;
			throw err;
		}
	}
}
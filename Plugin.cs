using BepInEx;
using BepInEx.Logging;
using AGM = DDoor.AlternativeGameModes;
using HL = HarmonyLib;

namespace ArchipelagoRandomizer;

[BepInPlugin("deathsdoor.archipelagorandomizer", "ArchipelagoRandomizer", "1.0.0.0")]
[BepInDependency("deathsdoor.itemchanger"), BepInDependency("deathsdoor.alternativegamemodes")]
public class Plugin : BaseUnityPlugin
{
	internal static new ManualLogSource Logger;

	private void Awake()
	{
		// Plugin startup logic
		Logger = base.Logger;
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

		AGM.AlternativeGameModes.Add("Archipelago Rando", () =>
		{
			new ItemRandomizer();
		});
	}
}
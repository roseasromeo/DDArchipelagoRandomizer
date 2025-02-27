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
			Logger = base.Logger;
			Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

			AGM.AlternativeGameModes.Add("Archipelago", () =>
			{
				StartGame();
			});

			InitStatus = 1;
		}
		catch (System.Exception err)
		{
			InitStatus = 2;
			throw err;
		}
	}

	private void StartGame()
	{
		// TODO: Remove static data when we can configure this in game
		Archipelago.APConnectionInfo connectionInfo = new()
		{
			URL = "localhost",
			Port = 38281,
			SlotName = "test",
			Password = ""
		};

		try
		{
			// Once connected, start item randomizer
			if (Archipelago.Instance.Connect(connectionInfo) != null)
			{
				ItemRandomizer.Instance.OnFileStarted();
			}
		}
		catch (LoginValidationException ex)
		{
			// TODO: Find a way to reset the title screen UI to allow re-entering file without restarting game
			// Currently, it'll start a vanilla file without informing user of failed connection
			Logger.LogError(ex.Message);
		}
	}
}
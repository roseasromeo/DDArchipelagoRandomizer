using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DDoor.ArchipelagoRandomizer;

internal class ArchipelagoRandomizerMod
{
	private static readonly ArchipelagoRandomizerMod instance = new();
	private GameObject archipelagoRandomizer;
	private bool hasInited;

	public static ArchipelagoRandomizerMod Instance => instance;

	private ArchipelagoRandomizerMod() { }

	/// <summary>
	/// Called only when an Archipelago save file is created. Useful for saving data to save file.
	/// </summary>
	public void OnFileCreated()
	{
		GameSave.currentSave.SetKeyState("ArchipelagoRandomizer", true);

		InitMod();
		EnableMod();
	}

	/// <summary>
	/// Initializes the mod. This runs the first time an Archipelago save file is loaded in a session.
	/// </summary>
	public void InitMod()
	{
		SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
		{
			if (scene.name == "TitleScreen")
			{
				DisableMod();
			}
		};

		hasInited = true;
	}

	/// <summary>
	/// Enables the mod. This runs every time an Archipelago save file is loaded.
	/// </summary>
	private void EnableMod()
	{
		if (!hasInited)
		{
			InitMod();
		}

		// TODO: Remove static data when we can configure this in game
		Archipelago.APConnectionInfo connectionInfo = new()
		{
			URL = "localhost",
			Port = 54762,
			SlotName = "test",
			Password = ""
		};

		try
		{
			// Once connected, start item randomizer
			if (Archipelago.Instance.Connect(connectionInfo) != null)
			{
				archipelagoRandomizer = new GameObject("ArchipelagoRandomizer");
				ItemRandomizer itemRando = archipelagoRandomizer.AddComponent<ItemRandomizer>();
				archipelagoRandomizer.AddComponent<DeathManager>();
				Object.DontDestroyOnLoad(archipelagoRandomizer);
			}
		}
		catch (LoginValidationException ex)
		{
			// TODO: Find a way to reset the title screen UI to allow re-entering file without restarting game
			// Currently, it'll start a vanilla file without informing user of failed connection
			Logger.LogError(ex.Message);
		}
	}

	/// <summary>
	/// Disables the mod. This runs every time you load into TitleScreen
	/// </summary>
	private void DisableMod()
	{
		Object.Destroy(archipelagoRandomizer);
	}

	[HarmonyPatch]
	private class Patches
	{
		/// <summary>
		/// Enables the mod if a compatible save file is loaded
		/// </summary>
		[HarmonyPrefix, HarmonyPatch(typeof(SaveSlot), nameof(SaveSlot.useSaveFile))]
		private static void LoadFilePatch(SaveSlot __instance)
		{
			GameSave.currentSave = __instance.saveFile;

			if (__instance.saveFile.IsLoaded() && GameSave.currentSave.IsKeyUnlocked("ArchipelagoRandomizer"))
			{
				Instance.EnableMod();
			}
		}
	}
}
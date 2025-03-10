using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DDoor.ArchipelagoRandomizer.Archipelago;
using AGM = DDoor.AlternativeGameModes;

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
	}

	/// <summary>
	/// Initializes the mod. This runs the first time an Archipelago save file is loaded in a session.
	/// </summary>
	public void InitMod()
	{
		if (hasInited)
		{
			return;
		}

		SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
		{
			if (SceneManager.GetActiveScene().name == "TitleScreen" && scene.name != "TitleScreen")
			{
				DisableMod();
			}
		};

		hasInited = true;
	}

	/// <summary>
	/// Enables the mod. This runs every time an Archipelago save file is loaded.
	/// </summary>
	public void EnableMod(APConnectionInfo connectionInfo, int saveInfoToSlotIndex)
	{
		if (!hasInited)
		{
			InitMod();
		}

		ConnectToArchipelago(connectionInfo, saveInfoToSlotIndex);
	}

	public void EnableMod(int saveInfoToSlotIndex)
	{
		APConnectionInfo connectionInfo = Archipelago.Instance.GetConnectionInfoForFile(saveInfoToSlotIndex);
		ConnectToArchipelago(connectionInfo, saveInfoToSlotIndex, isAlreadyLoading: true);
	}

	private async void ConnectToArchipelago(APConnectionInfo connectionInfo, int saveInfoToSlotIndex = 0, bool isAlreadyLoading = false)
	{
		try
		{
			// Once connected, start item randomizer
			if (await Archipelago.Instance.Connect(connectionInfo, saveInfoToSlotIndex) != null)
			{
				archipelagoRandomizer = new GameObject("ArchipelagoRandomizer");
				ItemRandomizer itemRando = archipelagoRandomizer.AddComponent<ItemRandomizer>();
				archipelagoRandomizer.AddComponent<DeathManager>();
				Object.DontDestroyOnLoad(archipelagoRandomizer);

				if (!isAlreadyLoading)
				{
					ConnectionMenu.Instance.HideAPMenuAndStartGame();
				}
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
		Archipelago.Instance.Disconnect();
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
				// If loading AP file
				if (AGM.AlternativeGameModes.SelectedModeName == "START")
				{
					instance.EnableMod(TitleScreen.instance.saveMenu.index);
				}
			}
		}
	}
}
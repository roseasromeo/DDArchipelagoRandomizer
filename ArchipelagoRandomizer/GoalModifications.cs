using DDoor.AddUIToOptionsMenu;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Utility;
using IC = DDoor.ItemChanger;

namespace DDoor.ArchipelagoRandomizer;

#nullable enable
internal class GoalModifications : MonoBehaviour
{
    private static GoalModifications? instance;
    public static GoalModifications? Instance => instance;
    private Goal? goal;
    private Goal GetGoal()
    {
        goal ??= Archipelago.Instance.GetSlotData<long>("goal") switch
        {
            0 => Goal.LordOfDoors,
            1 => Goal.TrueEnding,
            2 => Goal.GreenTablet,
            10 => Goal.Any,
            _ => Goal.LordOfDoors,
        };
        return (Goal)goal;
    }
    internal bool IsLordOfDoorsGoal()
    {
        return GetGoal() == Goal.LordOfDoors || GetGoal() == Goal.Any;
    }
    internal bool IsTrueEndingGoal()
    {
        return GetGoal() == Goal.TrueEnding || GetGoal() == Goal.Any;
    }
    internal bool IsGreenTabletGoal()
    {
        return GetGoal() == Goal.GreenTablet || GetGoal() == Goal.Any;
    }

    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += ModifyGoalScenes;
    }

    internal void ModifyGoalScenes(Scene scene, LoadSceneMode _)
    {
        string truthRuinsScene = "lvlConnect_Fortress_Mountaintops";
        if (scene.name == truthRuinsScene)
        {
            if (!IsTrueEndingGoal())
            {
                // Game appears to already disable if don't have all 7 tablets
                PathUtil.GetByPath(truthRuinsScene, "SceneMover/R_TruthRuins/_CONTENTS/TRUTH/_Trigger").SetActive(false);
            }
        }
    }

    [HarmonyPatch]
    private class Patches
    {
        /// <summary>
        /// Sends completion for main ending if that is the goal setting
        /// </summary>
        [HarmonyPrefix, HarmonyPatch(typeof(LodBoss2), nameof(LodBoss2.NextPhase))]
        private static void EndGameCsPatch(LodBoss2 __instance)
        {
            if (Archipelago.Instance.IsConnected())
            {
                if (__instance.deathCutscene && Instance != null && Instance.IsLordOfDoorsGoal())
                {
                    Archipelago.Instance.SendCompletion();
                }
            }
        }

        /// <summary>
        /// Sends completion for the true ending if that is the goal setting
        /// </summary>
        [HarmonyPrefix, HarmonyPatch(typeof(ActivateTrigger), nameof(ActivateTrigger.DoActivateTrigger))]
        private static void TrueEndingCsPatch(ActivateTrigger __instance)
        {
            if (Archipelago.Instance.IsConnected())
            {
                if (Instance != null && Instance.IsTrueEndingGoal())
                {
                    if (__instance.target.name == "TRUTH")
                    {
                        Archipelago.Instance.SendCompletion();
                    }
                }
            }
        }
    }

    private enum Goal
    {
        LordOfDoors,
        TrueEnding,
        GreenTablet,
        Any,
    }

    internal readonly struct GoalItem : IC.Item
	{
		public string DisplayName { get; }
		public string Icon { get; }
		public string Location { get; }

		public void Trigger()
		{
			Archipelago.Instance.SendCompletion();
		}

		public GoalItem(string displayName, string icon, string location)
		{
			DisplayName = displayName;
			Icon = icon;
			Location = location;
		}
	}
}
#nullable disable
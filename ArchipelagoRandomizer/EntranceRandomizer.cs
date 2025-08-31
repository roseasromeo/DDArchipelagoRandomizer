using HarmonyLib;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace DDoor.ArchipelagoRandomizer;

#nullable enable
internal class EntranceRandomizer : MonoBehaviour
{
    private static EntranceRandomizer? instance;
    public static EntranceRandomizer? Instance => instance;
    private Dictionary<string, string>? entrancePairings = null;

    internal Dictionary<string, string>? GetEntrancePairings()
    {
        JObject eP = (JObject)Archipelago.Instance.GetSlotData<object>("entrance_pairings");
        entrancePairings ??= eP.ToObject<Dictionary<string,string>>();
        return entrancePairings;
    }

    private void Awake()
    {
        instance = this;
    }

    [HarmonyPatch]
    private class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorTrigger), nameof(DoorTrigger.OnTriggerEnter))]
        private static bool PreOnTriggerEnter(DoorTrigger __instance, Collider collider)
        {
            SceneTransitions.SceneTransition? sceneTransition = SceneTransitions.GetConnectedSceneTransition(__instance.doorId, __instance.sceneToLoad);
            if (sceneTransition != null && !__instance.triggered)
            {
                __instance.doorId = sceneTransition?.loadingZoneId;
                __instance.sceneToLoad = sceneTransition?.sceneName;
                __instance.targetDoor = sceneTransition?.loadingZoneId;
            }
            Plugin.Logger.LogDebug($"doorId: {__instance.doorId}, sceneToLoad: {__instance.sceneToLoad}, targetDoor: {__instance.targetDoor}, parentDoor exists: {__instance.parentDoor != null}");
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ForestBuggy), nameof(ForestBuggy.Awake))]
        private static void PreAwake(ForestBuggy __instance)
        {
            SceneTransitions.SceneTransition? sceneTransition = SceneTransitions.GetConnectedSceneTransition(__instance.doorId, __instance.targetScene);
            if (sceneTransition != null)
            {
                __instance.doorId = sceneTransition?.loadingZoneId;
                __instance.targetScene = sceneTransition?.sceneName;
            }
        }
    }
}
#nullable disable
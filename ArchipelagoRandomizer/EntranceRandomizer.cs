using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DDoor.ArchipelagoRandomizer;

#nullable enable
internal class EntranceRandomizer : MonoBehaviour
{
    private static EntranceRandomizer? instance;
    public static EntranceRandomizer? Instance => instance;
    private bool modifyingDoorTrigger = false;

    private Dictionary<string, string>? entrancePairings = null;

    internal Dictionary<string, string>? GetEntrancePairings()
    {
        JObject eP = (JObject)Archipelago.Instance.GetSlotData<object>("entrance_pairings");
        entrancePairings ??= eP.ToObject<Dictionary<string, string>>();
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
            if (Archipelago.Instance.IsConnected() && Instance != null && !Instance.modifyingDoorTrigger)
            {
                Plugin.Logger.LogDebug($"Starting state: doorId: {__instance.doorId}, sceneToLoad: {__instance.sceneToLoad}, targetDoor: {__instance.targetDoor}");
                SceneTransitions.SceneTransition? oldSceneTransition = SceneTransitions.GetThisSceneTransition(__instance.doorId, __instance.sceneToLoad);
                SceneTransitions.SceneTransition? newSceneTransition = SceneTransitions.GetConnectedSceneTransition(__instance.doorId, __instance.sceneToLoad);
                PlayerInputControl playerInputControl = collider.gameObject.GetComponent<PlayerInputControl>();
                if (newSceneTransition != null && !__instance.triggered && playerInputControl != null)
                {
                    if (Instance != null)
                    {
                        Instance.modifyingDoorTrigger = true;
                    }
                    __instance.doorId = newSceneTransition?.loadingZoneId;
                    __instance.sceneToLoad = newSceneTransition?.originSceneName;
                    __instance.targetDoor = newSceneTransition?.loadingZoneId;
                    if (newSceneTransition?.originSceneName == oldSceneTransition?.originSceneName)
                    {
                        List<DoorTrigger> doorTriggers = ComponentUtil.FindAllComponentsOfTypeInScene<DoorTrigger>(newSceneTransition?.originSceneName);
                        Logger.LogList(doorTriggers);
                        try
                        {
                            DoorTrigger newDoor = doorTriggers.First(dt => dt.doorId == newSceneTransition?.loadingZoneId);
                            newDoor.parentDoor.gameObject.SetActive(true);
                            newDoor.teleportThroughThisDoor();
                            return false;
                        }
                        catch (InvalidOperationException)
                        {
                            Plugin.Logger.LogDebug($"No DoorTrigger with doorId {newSceneTransition?.loadingZoneId} found in scene {newSceneTransition?.originSceneName}.");
                        }
                    }
                }
                Plugin.Logger.LogDebug($"Ending State: doorId: {__instance.doorId}, sceneToLoad: {__instance.sceneToLoad}, targetDoor: {__instance.targetDoor}");
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorTrigger), nameof(DoorTrigger.OnTriggerExit))]
        private static void PreOnTriggerExit()
        {
            if (Instance != null)
            {
                Instance.modifyingDoorTrigger = false;
            }
            PlayerGlobal.instance.UnPauseInput();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ForestBuggy), nameof(ForestBuggy.Awake))]
        private static void PreAwake(ForestBuggy __instance)
        {
            if (Archipelago.Instance.IsConnected())
            {
                SceneTransitions.SceneTransition? sceneTransition = SceneTransitions.GetConnectedSceneTransition(__instance.doorId, __instance.targetScene);
                if (sceneTransition != null)
                {
                    __instance.doorId = sceneTransition?.loadingZoneId;
                    __instance.targetScene = sceneTransition?.toSceneName;
                }
            }
        }
    }
}
#nullable disable
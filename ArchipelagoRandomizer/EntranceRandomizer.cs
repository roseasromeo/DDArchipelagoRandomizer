using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DDoor.ArchipelagoRandomizer;

#nullable enable
internal class EntranceRandomizer : MonoBehaviour
{
    private static EntranceRandomizer? instance;
    public static EntranceRandomizer? Instance => instance;
    private bool enteringHallOfDoors = false;
    private bool loadingIn = true;

    private Dictionary<string, string>? entrancePairings = null;

    internal Dictionary<string, string>? GetEntrancePairings()
    {
        if (entrancePairings == null)
        {
            JObject eP = (JObject)Archipelago.Instance.GetSlotData<object>("entrance_pairings");
            entrancePairings = eP.ToObject<Dictionary<string, string>>();
        }
        return entrancePairings;
    }

    private void Awake()
    {
        instance = this;
        SceneManager.sceneLoaded += RestorePlayerControl;
    }

    private void Destroy()
    {
        SceneManager.sceneLoaded -= RestorePlayerControl;
    }

    private void RestorePlayerControl(Scene scene, LoadSceneMode _)
    {
        if (SceneTransitions.IsSceneInRandomizedTransitions(scene.name))
        {
            PlayerGlobal.instance?.UnPauseInput_Cutscene();
        }
    }

    [HarmonyPatch]
    private class Patches
    {
        [HarmonyPatch(typeof(ShortcutDoor), nameof(ShortcutDoor.Awake))]
        [HarmonyPostfix]
        private static void PostShortcutDoorAwake(ShortcutDoor __instance)
        {
            DoorTrigger doorTrigger = __instance.doorTrigger;
            SceneTransitions.SceneTransition? newSceneTransition = SceneTransitions.GetConnectedSceneTransition(doorTrigger.doorId, doorTrigger.sceneToLoad);
            doorTrigger.doorId = newSceneTransition?.loadingZoneId;
            doorTrigger.sceneToLoad = newSceneTransition?.toSceneName;
            doorTrigger.targetDoor = newSceneTransition?.loadingZoneId;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorTrigger), nameof(DoorTrigger.OnTriggerEnter))]
        private static bool PreOnTriggerEnter(DoorTrigger __instance, Collider collider)
        {
            if (__instance.parentDoor == null) // All Shortcut Doors will have been modified on Awake
            {
                if (Archipelago.Instance.IsConnected() && Instance != null)
                {
                    SceneTransitions.SceneTransition? newSceneTransition = SceneTransitions.GetConnectedSceneTransition(__instance.doorId, __instance.sceneToLoad);
                    PlayerInputControl playerInputControl = collider.gameObject.GetComponent<PlayerInputControl>();
                    if (newSceneTransition != null && !__instance.triggered && playerInputControl != null)
                    {
                        __instance.doorId = newSceneTransition?.loadingZoneId;
                        __instance.sceneToLoad = newSceneTransition?.toSceneName;
                        __instance.targetDoor = newSceneTransition?.loadingZoneId;
                    }
                }
            }
            if (__instance.targetDoor != null && __instance.targetDoor.Contains("sdoor_") && __instance.sceneToLoad != null && __instance.sceneToLoad.Contains("hallofdoors"))
            {
                // If entering Hall of Doors from a shortcut door, it breaks if you load in directly, so set a flag so that we can handle it
                if (Instance != null)
                {
                    Instance.enteringHallOfDoors = true;
                }
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorTrigger), nameof(DoorTrigger.OnTriggerExit))]
        private static void PostOnTriggerExit()
        {
            PlayerGlobal.instance.UnPauseInput_Cutscene();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorTrigger), nameof(DoorTrigger.Awake))]
        private static bool PreDoorTriggerAwake(DoorTrigger __instance)
        {
            // Prevent breaking when loading into Hall of Doors/from a save
            if (Instance != null && (Instance.enteringHallOfDoors || Instance.loadingIn) && __instance.doorId != "" && !GameSave.GetSaveData().IsKeyUnlocked(DoorTrigger.currentTargetDoor))
            {
                if (__instance.doorId == DoorTrigger.currentTargetDoor)
                {
                    if (PlayerGlobal.instance != null)
                    {
                        if (__instance.spawnPoint == null)
                        {
                            __instance.spawnPoint = __instance.gameObject.transform.GetChild(0);
                        }
                        PlayerGlobal.instance.SetPosition(__instance.spawnPoint.position, false, false);
                        PlayerGlobal.instance.SetSafePos(__instance.spawnPoint.position);
                        PlayerGlobal.instance.SetRotation(__instance.spawnPoint.rotation);
                        DoorTrigger.currentTargetDoor = "";
                        PlayerGlobal.instance.UnPauseInput();
                        PlayerGlobal.instance.UnPauseInput_Cutscene();
                        __instance.parentDoor = __instance.gameObject.GetComponentInParent<ShortcutDoor>();
                        Instance.loadingIn = false;
                        Instance.enteringHallOfDoors = false;
                        return false;
                    }
                    else
                    {
                        __instance.retryInStart = true;
                    }
                }
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorTrigger), nameof(DoorTrigger.Start))]
        private static bool PreDoorTriggerStart(DoorTrigger __instance)
        {
            // Prevent breaking when loading into Hall of Doors/from a save
            if (Instance != null && (Instance.enteringHallOfDoors && __instance.retryInStart || Instance.loadingIn))
            {
                if (PlayerGlobal.instance != null)
                {
                    if (__instance.spawnPoint == null)
                    {
                        __instance.spawnPoint = __instance.gameObject.transform.GetChild(0);
                    }
                    PlayerGlobal.instance.SetPosition(__instance.spawnPoint.position, false, false);
                    PlayerGlobal.instance.SetSafePos(__instance.spawnPoint.position);
                    PlayerGlobal.instance.SetRotation(__instance.spawnPoint.rotation);
                    DoorTrigger.currentTargetDoor = "";
                    PlayerGlobal.instance.UnPauseInput();
                    PlayerGlobal.instance.UnPauseInput_Cutscene();
                    __instance.parentDoor = __instance.gameObject.GetComponentInParent<ShortcutDoor>();
                    Instance.enteringHallOfDoors = false;
                    Instance.loadingIn = false;
                    return false;
                }
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ForestBuggy), nameof(ForestBuggy.Awake))]
        private static void PreForestBuggyAwake(ForestBuggy __instance)
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
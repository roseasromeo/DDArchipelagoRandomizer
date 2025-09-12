using DDoor.AddUIToOptionsMenu;
using DDoor.ItemChanger;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace DDoor.ArchipelagoRandomizer;

public class TrapManager : MonoBehaviour
{
    private static TrapManager instance;
    public static TrapManager Instance => instance;

    public Dictionary<string, TrapType> traps = new() {
        {"Rotation Trap", TrapType.Rotation },
        {"Invisibility Trap", TrapType.Invisibility },
    };
    internal IEnumerator trapHandler;
    internal ConcurrentQueue<TrapType> trapQueue;
    private readonly float trapDelay = 3f;

    private void Awake()
    {
        instance = this;
        trapQueue = new ConcurrentQueue<TrapType>();
        trapHandler = TrapHandler();
    }

    private IEnumerator TrapHandler()
    {
        while (Archipelago.Instance.IsConnected())
        {
            if (!trapQueue.TryPeek(out TrapType trap))
            {
                yield return true;
                continue;
            }

            switch (trap)
            {
                case TrapType.Rotation:
                    RotationTrap(); break;
                case TrapType.Invisibility:
                    InvisibilityTrap(); break;
            }
            trapQueue.TryDequeue(out _);
            // Add delay between each trap
            float timePreDelay = Time.time;
            while (Time.time - timePreDelay < trapDelay)
            {
                yield return null;
            }
        }
    }

    private float accumulatedAngle = 0f;
    private void RotationTrap()
    {
        accumulatedAngle += 90f;
        CameraRotationControl.instance.Rotate(accumulatedAngle);
    }

    private void InvisibilityTrap()
    {
        Transform playerVisuals = PlayerGlobal.instance.playerVisuals;

        string basePath = "PlayerContents/VISUALS/crow_player (fbx)/";

        PathUtil.GetByPath("_PLAYER", basePath + "body").SetActive(false);
        PathUtil.GetByPath("_PLAYER", basePath + "eyes").SetActive(false);
        PathUtil.GetByPath("_PLAYER", basePath + "tail").SetActive(false);
        switch (GameSave.GetSaveData().GetWeaponId())
        {
            case "sword":
                PathUtil.GetByPath("_PLAYER", basePath + "WEAPON_Sword(Clone)/Model").SetActive(false); break;
            case "hammer":
                PathUtil.GetByPath("_PLAYER", basePath + "WEAPON_Hammer(Clone)/Hammer").SetActive(false); break;
            case "daggers":
                PathUtil.GetByPath("_PLAYER", basePath + "WEAPON_Daggers(Clone)/Model1").SetActive(false);
                PathUtil.GetByPath("_PLAYER", basePath + "WEAPON_Daggers(Clone)/Model2").SetActive(false); break;
            case "sword_heavy":
                PathUtil.GetByPath("_PLAYER", basePath + "WEAPON_Greatsword(Clone)/Model").SetActive(false); break;
            case "umbrella":
                PathUtil.GetByPath("_PLAYER", basePath + "WEAPON_Umbrella(Clone)/Model").SetActive(false); break;
        }
    }



    public enum TrapType
    {
        Rotation,
        Invisibility
    }
}
using DDoor.AddUIToOptionsMenu;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AMC = Archipelago.MultiClient.Net;
using IC = DDoor.ItemChanger;

namespace DDoor.ArchipelagoRandomizer;

internal class ModelSwapper : MonoBehaviour
{
    private static ModelSwapper instance;
    internal static ModelSwapper Instance => instance;
    private Dictionary<string, ItemData[]> itemsToCache;
    private readonly List<string> itemLookup = [];
    private readonly Dictionary<string, LocationData[]> locationsToSwap = new()
    {
        {
            "lvl_HallOfDoors", [
                new LocationData{ itemChangerName = "Discarded Umbrella", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Ancient Door Scale Model", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Modern Door Scale Model", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Rusty Belltower Key", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Surveillance Device", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Fire Return Upper", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Fire Return Lower", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Hookshot Secret", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Bomb Return", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Bomb Secret", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Hookshot Return", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Fire Secret", locationType = LocationType.DropItem },
            ]
       },
       {
            "lvl_Tutorial", [
                new LocationData{ itemChangerName = "Makeshift Soul Key", locationType = LocationType.DropItem },
            ]
       },
       {
            "lvl_Graveyard", [
                new LocationData{ itemChangerName = "Heart Shrine-Cemetery Behind Entrance", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Magic Shrine-Cemetery After Smough Arena", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Heart Shrine-Cemetery Winding Bridge End", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Old Compass", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Incense", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Undying Blossom", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Cyan Ancient Tablet of Knowledge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Purple Ancient Tablet of Knowledge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Arrow Silent Servant", path = "R_CatacombsServant/_CONTENTS/UPGRADEDOOR_Arrows Variant/GameObject", locationType = LocationType.Other },
                new LocationData{ itemChangerName = "Bomb Silent Servant", path = "R_BombSecret_COLLIDERCHECK/_CONTENTS/UPGRADEDOOR_Bombs Variant/GameObject", locationType = LocationType.Other },
                new LocationData{ itemChangerName = "Soul Orb-Cemetery Winding Bridge End", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Catacombs Room 2", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Catacombs Room 1", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Catacombs Exit", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Cemetery Gated Sewer", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Cemetery Under Bridge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Cemetery East Tree", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Cemetery Hookshot Towers", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Cemetery Cave", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Catacombs Entrance", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Cemetery Broken Bridge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Catacombs Tower", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Cemetery Left of Main Entrance", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Cemetery Near Tablet Gate", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Key-Cemetery Grey Crow", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Key-Cemetery Central", locationType = LocationType.Key },
                //TODO EAST TREE SOUL ORB??
            ]
       },
       {
            "lvlConnect_Graveyard_Gardens", [
                new LocationData{ itemChangerName = "Soul Orb-Estate Access Crypt", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Fire Silent Servant", locationType = LocationType.Other },
            ]
       },
       {
            "lvl_GrandmaGardens", [
                new LocationData{ itemChangerName = "Sludge-Filled Urn", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Rusty Garden Trowel", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Green Ancient Tablet of Knowledge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Estate Owl", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Rogue Daggers", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Garden of Love Turncam", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Garden of Life Hookshot Loop", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Garden of Love Bomb Walls", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Garden of Life Bomb Wall", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Garden of Peace", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Estate Balcony", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Estate Broken Boardwalk", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Estate Sewer Middle", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Estate Sewer End", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Estate Secret Cave", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Estate Twin Benches", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Magic Shrine-Estate Left of Manor", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Seed-Estate Family Tomb", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Garden of Joy", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Estate Entrance", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Estate Hedge Gaps", locationType = LocationType.DropItem },
            ]
       },
       {
            "lvl_GrandmaMansion", [
                new LocationData{ itemChangerName = "Key-Manor Library", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Key-Manor After Spinny Pot Room", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Key-Manor Under Dining Room", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Old Photograph", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Engagement Ring", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Crow-Manor Imp Loft", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Manor Library", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Manor Bedroom", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Manor After Torch Puzzle", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Soul Orb-Manor Library Shelf", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Manor Imp Loft", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Manor Chandelier", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Manor Chandelier", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Manor Main Room Upper", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Manor Library Shelf", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Manor Spinny Pot Room", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Manor Near Brazier",locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Manor Boxes", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Manor Below Big Pot Arena", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Manor Rafters", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Magic Shrine-Manor Bathroom Puzzle", locationType = LocationType.Shrine },
            ]
       },
       {
            "lvlconnect_Mansion_Basement", [
                new LocationData{ itemChangerName = "Soul Orb-Furnace Entrance Sewer", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Small Room", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Furnace Lantern Chain", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Furnace Entrance", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Cart Puzzle", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Heart Shrine-Furnace Cart Puzzle", locationType = LocationType.DropItem },
            ]
       },
       {
            "lvl_GrandmaBasement", [
                new LocationData{ itemChangerName = "Seed-Inner Furnace Horizontal Pistons", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Inner Furnace Conveyor Bridge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Inner Furnace Conveyor Gauntlet", locationType = LocationType.DropItem },
            ]
       },
       {
            "lvl_Forest", [
                new LocationData{ itemChangerName = "Malformed Seed", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Magical Forest Horn", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Corrupted Antler", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Thunder Hammer", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Yellow Ancient Tablet of Knowledge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Ruins Owl", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Key-Overgrown Ruins", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Key-Dungeon Hall", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Key-Dungeon Right", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Key-Dungeon Near Water Arena", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Soul Orb-Dungeon Vine", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Stump", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Outside Left Dungeon Exit", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Dungeon Cobweb", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Left After Key Door", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Lower Bomb Wall", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Lord of Doors Arena Hookshot", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Dungeon Lower Entrance", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Upper Above Horn", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Above Three Plants", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Up Turncam Ladder", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Above Entrance Gate", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Upper Bomb Wall", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Dungeon Left Exit Shelf", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Ruins Lower Hookshot", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Dungeon Fire Puzzle Near Water Arena", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Ruins Lord of Doors Arena", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Ruins Fire Plant Corridor", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Dungeon Water Arena Left Exit", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Ruins Right Middle", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Ruins On Settlement Wall", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Ruins Behind Boxes", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Ruins Down Through Bomb Wall", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Dungeon Above Rightmost Crow", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Dungeon Right Above Key", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Dungeon Avarice Bridge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Crow-Dungeon Hall", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Dungeon Water Arena", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Dungeon Cobweb", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Dungeon Rightmost", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Magic Shrine-Ruins Hookshot Arena", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Magic Shrine-Ruins Turncam", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Heart Shrine-Dungeon Water Arena", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Heart Shrine-Ruins Sewer", locationType = LocationType.Shrine },
            ]
        },
        {
            "lvl_Swamp", [
                new LocationData{ itemChangerName = "Red Ancient Tablet of Knowledge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Fortress Watchtower", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Fortress Statue", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Fortress Bridge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Fortress Intro Crate", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Fortress East After Statue", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Fortress Bomb", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Fortress Hidden Sewer", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Fortress Drop", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Magic Shrine-Fortress Bow Secret", locationType = LocationType.Shrine },
            ]
       },
       {
            "boss_Frog", [
                new LocationData{ itemChangerName = "Giant Arrowhead", locationType = LocationType.DropItem },
            ]
       },
       {
            "lvlConnect_Graveyard_Sailor", [
                new LocationData{ itemChangerName = "Token of Death", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Between Cemetery and Sailor", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Hookshot Silent Servent", path = "HookshotUpgradeRoom/_CONTENTS/UPGRADEDOOR_Hookshot Variant/GameObject", locationType = LocationType.Other },
            ]
       },
       {
            "lvl_SailorMountain", [
                new LocationData{ itemChangerName = "Captain's Log", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Ink-Covered Teddy Bear", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Grunt's Old Mask", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Reaper's Greatsword", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Sailor Upper", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Sailor Turncam", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Heart Shrine-Hookshot Arena", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Magic Shrine-Sailor Turncam", locationType = LocationType.Shrine },
            ]
       },
       {
            "lvl_FrozenFortress", [
                new LocationData{ itemChangerName = "Ancient Crown", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Death's Contract", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Lockstone Upper East", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Lockstone Soul Door", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Lockstone Behind Bars", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Lockstone Entrance West", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Lockstone North", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-North Lockstone Sewer", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Lockstone Hookshot North", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Lockstone Exit", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-West Lockstone Sewer", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Key-Lockstone West", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Key-Lockstone North", locationType = LocationType.Key },
                new LocationData{ itemChangerName = "Crow-Lockstone East", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Lockstone West", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Lockstone West Locked", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Crow-Lockstone South West", locationType = LocationType.Crow },
                new LocationData{ itemChangerName = "Magic Shrine-Lockstone Secret West", locationType = LocationType.Shrine },
            ]
       },
       {
            "lvlConnect_Fortress_Mountaintops", [
                new LocationData{ itemChangerName = "Soul Orb-Camp Rooftops", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Camp Broken Bridge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Camp Ledge With Huts", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Camp Stall", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Camp Rooftops", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Shiny Medallion", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Heart Shrine-Camp Ice Skating", locationType = LocationType.Shrine },
                new LocationData{ itemChangerName = "Key-Camp of the Free Crows", locationType = LocationType.Key },
            ]
       },
       {
            "lvl_mountaintops", [
                new LocationData{ itemChangerName = "Mysterious Locket", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Blue Ancient Tablet of Knowledge", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Watchtowers Owl", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Soul Orb-Watchtowers Behind Barb Elevator", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Watchtowers Ice Skating", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Watchtowers Tablet Door", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Watchtowers Archer Platform", locationType = LocationType.DropItem },
                new LocationData{ itemChangerName = "Seed-Watchtowers Boxes", locationType = LocationType.DropItem },
            ]
       },
    };
    private readonly Dictionary<string, float> boostAmounts = new() {
        {
            "Discarded Umbrella", 1f
        },
    };

    private readonly Dictionary<string, Dictionary<LocationData, GameObject>> preloadedSwaps = [];

    internal enum LocationType
    {
        DropItem,
        Crow,
        Shrine,
        Key,
        Other,
    }

    internal enum ItemType
    {
        Fire,
        Bomb,
        Hookshot,
        Bow,
        Sword,
        RogueDaggers,
        DiscardedUmbrella,
        ReapersGreatsword,
        ThunderHammer,
        VitalityShard,
        MagicShard,
        LifeSeed,
        SoulOrb,
        PinkKey,
        GreenKey,
        YellowKey,
        EngagementRing,
        OldCompass,
        Incense,
        UndyingBlossom,
        OldPhotograph,
        SludgeFilledUrn,
        TokenOfDeath,
        RustyGardenTrowel,
        CaptainsLog,
        GiantArrowhead,
        MalformedSeed,
        CorruptedAntler,
        MagicalForestHorn,
        AncientCrown,
        GruntsOldMask,
        AncientDoorScaleModel,
        ModernDoorScaleModel,
        RustyBelltowerKey,
        SurveillanceDevice,
        ShinyMedallion,
        InkCoveredTeddyBear,
        DeathsContract,
        MakeshiftSoulKey,
        MysteriousLocket,
        RedAncientTabletOfKnowledge,
        YellowAncientTabletOfKnowledge,
        GreenAncientTabletOfKnowledge,
        CyanAncientTabletOfKnowledge,
        BlueAncientTabletOfKnowledge,
        PurpleAncientTabletOfKnowledge,
        PinkAncientTabletOfKnowledge,
        Crow,
        Lever
    }

    private void Awake()
    {
        instance = this;
        itemsToCache = new()
        {
            {
                "lvl_HallOfDoors", [
                    new ItemData { type = ItemType.RustyBelltowerKey, name  = "Rusty Belltower Key", path  = "ENDGAME_STATE_CONTROL/_END_GAME_STATE/TRINKET_RustyKey Variant/Visuals/rustyKey"},
                    new ItemData { type = ItemType.SurveillanceDevice, name  = "Surveillance Device", path  = "TRINKET_Surveillance Variant/Visuals/gramophoneBase"},
                    new ItemData { type = ItemType.ModernDoorScaleModel, name  = "Modern Door Scale Model", path  = "TRINKET_ProtoDoor_new Variant/Visuals/ShortcutDoor"},
                    new ItemData { type = ItemType.AncientDoorScaleModel, name  = "Ancient Door Scale Model", path  = "TRINKET_ProtoDoor_old Variant/Visuals/ancientDoor"},
                    new ItemData { type = ItemType.DiscardedUmbrella, name  = "Discarded Umbrella", path  = "WEAPON_PICKUP_Umbrella/Visuals/Shiner/umbrella"},
                ]
            },
            {
                "lvl_Tutorial", [new ItemData { type = ItemType.MakeshiftSoulKey, name  = "Makeshift Soul Key", path  = "TRINKET_SoulKey Variant/Visuals/makeshiftKey"}]
            },
            {
                "lvl_Graveyard", [
                    new ItemData { type = ItemType.LifeSeed, name  = "Life Seed", path  = "MainRoom/_CONTENTS/LevelChunks/34/_CONTENTS/_DROP_seed (1)/Visuals/plant_seed"},
                    new ItemData { type = ItemType.SoulOrb, name  = "100 Souls", path  = "R_BridgeTowerRoom/_CONTENTS/_DROP_soul_pickup_100 Variant (4)/Visuals/Obj/Soul_Pickup"},
                    new ItemData { type = ItemType.PinkKey, name  = "Pink Key", path  = "MainRoom/_CONTENTS/LevelChunks/5/_CONTENTS/KeyShrine_GRAVE/Armature/_Root/Pillar/KEY_Grave/keyHover/graveKey"},
                    new ItemData { type = ItemType.PurpleAncientTabletOfKnowledge, name  = "Purple Ancient Tablet of Knowledge", path  = "R_GravediggerCrypt_Interior/_CONTENTS/SpawnTruthTablet"},
                    new ItemData { type = ItemType.CyanAncientTabletOfKnowledge, name  = "Cyan Ancient Tablet of Knowledge", path  = "R_TruthTablet/_CONTENTS/TRUTH_Tablet_4_LMA_Ghosts/Visuals/EyeStone1"},
                    new ItemData { type = ItemType.OldCompass, name  = "Old Compass", path  = "R_CatacombsStart/_CONTENTS/TRINKET_Compass Variant/Visuals/compass"},
                    new ItemData { type = ItemType.UndyingBlossom, name  = "Undying Blossom", path  = "R_Dovecote/_CONTENTS/TRINKET_Flower Variant/Visuals/greyFlower"},
                    new ItemData { type = ItemType.Incense, name  = "Incense", path  = "MainRoom/_CONTENTS/TRINKET_Incense Variant/Visuals/incense"},
                    new ItemData { type = ItemType.MagicShard, name  = "Magic Shard", path  = "MainRoom/_CONTENTS/SHRINE_Crow_Arrows Variant/BaseStatue/Anim/Tounge/ITEM_HeartContainer/UI_Visual (1)"},
                    new ItemData { type = ItemType.VitalityShard, name  = "Vitality Shard", path  = "R_SHRINE/_CONTENTS/SHRINE_Crow/BaseStatue/Anim/Tounge/ITEM_HeartContainer/UI_Visual"},
                    new ItemData { type = ItemType.Bow, name  = "Arrow Upgrade", path  = "R_CatacombsServant/_CONTENTS/UPGRADEDOOR_Arrows Variant/GameObject"},
                    new ItemData { type = ItemType.Bomb, name  = "Bomb", path  = "R_BombSecret_COLLIDERCHECK/_CONTENTS/UPGRADEDOOR_Bombs Variant/GameObject"},
                    new ItemData { type = ItemType.Lever, name  = "Lever", path  = "MainRoom/_CONTENTS/LevelChunks/6/_CONTENTS/Frog_Lever (4)"},
                ]
            },
            {
                "lvlConnect_Graveyard_Gardens", [new ItemData { type = ItemType.Fire, name  = "Fire", path  = "SceneMover/ROOM_Enter (1)/_CONTENTS/UPGRADEDOOR_Fire Variant/GameObject"}]
            },
            {
                "lvl_GrandmaGardens", [
                    new ItemData { type = ItemType.SludgeFilledUrn, name  = "Sludge-Filled Urn", path  = "R_ElixirShed/_CONTENTS/TRINKET_elixir Variant/Visuals/elixir"},
                    new ItemData { type = ItemType.RustyGardenTrowel, name  = "Rusty Garden Trowel", path  = "R_Gardens/_CONTENTS/TRINKET_trowel Variant (1)/Visuals/trowel"},
                    new ItemData { type = ItemType.GreenAncientTabletOfKnowledge, name  = "Green Ancient Tablet of Knowledge", path  = "R_TruthTablet/_CONTENTS/TruthStairs/TRUTH_Tablet_3_Plants/Visuals/EyeStone1"},
                    new ItemData { type = ItemType.PinkAncientTabletOfKnowledge, name  = "Pink Ancient Tablet of Knowledge", path  = "R_Gardens/_CONTENTS/NightOnly/OWL_CONTENTS_1/SpawnTruthShard_1"}, // Note that this is just one shard, but we give the player the three shards as one item
                    new ItemData { type = ItemType.RogueDaggers, name  = "Rogue Daggers", path  = "R_Gardens/_CONTENTS/WEAPON_PICKUP_Daggers/Visuals/Visuals"},
                ]
            },
            {
                "lvl_GrandmaMansion", [
                    new ItemData { type = ItemType.YellowKey, name  = "Yellow Key", path  = "_SCENE_MOVER/GroundFloor/R_LibrarySecretTrial/_CONTENTS/KeyShrine_MANSION/Armature/_Root/Pillar/KEY_Mansion/keyHover/mansionKey"},
                    new ItemData { type = ItemType.OldPhotograph, name  = "Old Photograph", path  = "_SCENE_MOVER/GroundFloor/SecretWood/_CONTENTS/TRINKET_photo Variant/Visuals/oldPhoto"},
                    new ItemData { type = ItemType.EngagementRing, name  = "Engagement Ring", path  = "_SCENE_MOVER/R_Walkway (1)/_CONTENTS/TRINKET_Ring Variant/Visuals/ring"},
                    new ItemData { type = ItemType.Crow, name  = "Crow", path  = "_SCENE_MOVER/R_Roof (1)/_CONTENTS/SOULKEY_AvariceDoor (1)/Soul/crow_2"},
                ]
            },
            {
                "lvl_Forest", [
                    new ItemData { type = ItemType.GreenKey, name  = "Green Key", path  = "_SceneMover/Room_ForestMain/_CONTENTS/ForestMain/Intro/Objects/KeyShrine_FOREST/Armature/_Root/Pillar/KEY_Forest/keyHover/forestKey"},
                    new ItemData { type = ItemType.MalformedSeed, name  = "Malformed Seed", path  = "_SceneMover/Room_ForestMain/_CONTENTS/TRINKET_seed Variant (1)/Visuals/seed"},
                    new ItemData { type = ItemType.MagicalForestHorn, name  = "Magical Forest Horn", path  = "_SceneMover/Room_ForestMain/_CONTENTS/TRINKET_basoon Variant/Visuals/basoon"},
                    new ItemData { type = ItemType.CorruptedAntler, name  = "Corrupted Antler", path  = "_SceneMover/Dungeon/Room_CentralRoute1/_CONTENTS/TRINKET_antler Variant/Visuals/CorruptedAntler"},
                    new ItemData { type = ItemType.YellowAncientTabletOfKnowledge, name  = "Yellow Ancient Tablet of Knowledge", path  = "_SceneMover/Room_ForestMain/_CONTENTS/_AVARICE_TruthSecret/SpawnTruthTablet"},
                    new ItemData { type = ItemType.ThunderHammer, name  = "Thunder Hammer", path  = "_SceneMover/Dungeon/Room_VerticalRoomNew/_CONTENTS/WEAPON_PICKUP_Hammer/GameObject/Visuals/hammerWeaponModel"},
                ]
            },
            {
                "lvl_Swamp", [new ItemData { type = ItemType.RedAncientTabletOfKnowledge, name  = "Red Ancient Tablet of Knowledge", path  = "JeffQuestDay/Section1EndPlatform (1)/TabletSpawner/SpawnTruthTablet"},]
            },
            {
                "boss_Frog", [new ItemData { type = ItemType.GiantArrowhead, name  = "Giant Arrowhead", path  = "_SceneMover/R_Cathedral_DOWN/_CONTENTS/TRINKET_arrow Variant/Visuals/arrowhead/Arrow/arrow"},]
            },
            {
                "lvlConnect_Graveyard_Sailor", [
                    new ItemData { type = ItemType.TokenOfDeath, name  = "Token of Death", path  = "R_SailorCaves1/_CONTENTS/TRINKET_coin Variant/Visuals/coin"},
                    new ItemData { type = ItemType.Hookshot, name  = "Hookshot", path  = "HookshotUpgradeRoom/_CONTENTS/UPGRADEDOOR_Hookshot Variant/GameObject"},
                ]
            },
            {
                "lvl_SailorMountain", [
                    new ItemData { type = ItemType.CaptainsLog, name  = "Captain's Log", path  = "R_Outside/_CONTENTS/TRINKET_journal Variant/Visuals/diary"},
                    new ItemData { type = ItemType.InkCoveredTeddyBear, name  = "Ink-Covered Teddy Bear", path  = "R_Outside/_CONTENTS/TRINKET_Teddy Variant/Visuals/Teddy"},
                    new ItemData { type = ItemType.GruntsOldMask, name  = "Grunt's Old Mask", path  = "R_Outside/_CONTENTS/OUTSIDE_CAMP/CleverGruntActivationLock/Grunt/TRINKET_FrogMask Variant/Visuals/mask"},
                    new ItemData { type = ItemType.ReapersGreatsword, name  = "Reaper's Greatsword", path  = "R_Outside/_CONTENTS/WEAPON_PICKUP_Greatsword/GameObject/Visuals/bigSwordHandle"},
                ]
            },
            {
                "lvl_FrozenFortress", [
                    new ItemData { type = ItemType.DeathsContract, name  = "Death's Contract", path  = "SceneMover/Hookshot_Dungeon/Ground_Floor/R_Library/_CONTENTS/TRINKET_contract Variant/Visuals/rollBody"},
                    new ItemData { type = ItemType.AncientCrown, name  = "Ancient Crown", path  = "SceneMover/Hookshot_Dungeon/Ground_Floor/NorthTower_Lower_New/_CONTENTS/TRINKET_AncientCrown Variant/Visuals/Head"},
                ]
            },
            {
                "lvlConnect_Fortress_Mountaintops", [new ItemData { type = ItemType.ShinyMedallion, name  = "Shiny Medallion", path  = "SceneMover/R_Storage/_CONTENTS/TRINKET_Medal Variant/Visuals/covenantMedal"},]
            },
            {
                "lvl_mountaintops", [
                    new ItemData { type = ItemType.MysteriousLocket, name  = "Mysterious Locket", path  = "TRINKET_locket Variant/Visuals/lockerBack"},
                    new ItemData { type = ItemType.BlueAncientTabletOfKnowledge, name  = "Blue Ancient Tablet of Knowledge", path  = "R_TruthTablet/_CONTENTS/TruthStairs/TRUTH_Tablet_5_WatchtowerTorches/Visuals/EyeStone1"},
                ]
            },
        };
        // Setup itemLookup
        foreach (ItemData[] dataArr in itemsToCache.Values)
        {
            foreach (ItemData data in dataArr)
            {
                if (!itemLookup.Contains(data.name))
                {
                    itemLookup.Add(data.name);
                }
            }
        }
    }

    private void OnEnable()
    {
        Logger.Log("Item Model Swap started");
        foreach (KeyValuePair<string, ItemData[]> kvp in itemsToCache)
        {
            string sceneName = kvp.Key;
            ItemData[] itemDatas = kvp.Value;

            Preloader.Instance.AddObjectToCacheList(sceneName, () =>
            {
                List<GameObject> items = [];
                foreach (ItemData itemData in itemDatas)
                {
                    GameObject item = FindGameObjectForItem(sceneName, itemData);
                    if (item == null)
                    {
                        Logger.LogError($"During item model preload, failed to find item\n    {itemData.name}\n    in {sceneName}");
                        continue;
                    }
                    item.name = itemData.name;
                    items.Add(item);
                }
                Logger.Log($"Finished caching items for scene {sceneName}!");
                return [.. items];
            }
            );
        }
        Preloader.Instance.OnPreloadDone += AfterPreload;
    }

    private void OnDisable()
    {
        Preloader.Instance.OnPreloadDone -= AfterPreload;
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void AfterPreload()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        foreach (string sceneName in locationsToSwap.Keys)
        {
            preloadedSwaps[sceneName] = [];
            foreach (LocationData locationData in locationsToSwap[sceneName])
            {
                if (!ItemRandomizer.Instance.TryGetICLocation(locationData.itemChangerName, out ItemRandomizer.DDItem? item))
                {
                    continue;
                }
                if (item != null)
                {
                    ItemRandomizer.DDItem dditem = (ItemRandomizer.DDItem)item;
                    string itemName;
                    if (dditem.DisplayName.Contains(" for "))
                    {
                        itemName = dditem.DisplayName.Split([" for "], StringSplitOptions.None)[^2];
                    }
                    else
                    {
                        itemName = dditem.DisplayName;
                    }
                    if (itemName.IndexOf("Lever", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        itemName = "Lever";
                    }
                    else if (itemName.IndexOf("Crow", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        itemName = "Crow";
                    }
                    else if (itemName.IndexOf("Trap", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        itemName = "Trap";
                    }
                    GameObject newModel;
                    if (itemLookup.Contains(itemName))
                    {
                        newModel = BaseModel(itemName);
                    }
                    else if (itemName == "Trap")
                    {
                        newModel = TrapModel();
                    }
                    else if (itemName.IndexOf(" Door", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        newModel = BaseModel("Modern Door Scale Model");
                    }
                    else if (itemName.IndexOf("Giant Soul", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        newModel = BaseModel("100 Souls");
                        newModel.transform.localScale = new Vector3(2,2,2);
                    }
                    else if (itemName == "Reaper's Sword")
                    {
                        newModel = BaseModel("Reaper's Greatsword"); // Since the Reaper's Sword is not a model out in the world
                    }
                    else if (ConvertOtherGameNamesToDeathsDoor(itemName, out string DDItemName))
                    {
                        newModel = BaseModel(DDItemName);
                    }
                    else
                    {
                        newModel = RecolorSouls((ItemRandomizer.ClassEnum)dditem.ItemClassification);
                    }
                    preloadedSwaps[sceneName][locationData] = newModel;
                }
            }
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode _)
    {
        Logger.Log(scene.name);
        List<LocationData> successfulSwaps = [];
        if (preloadedSwaps.Keys.Contains(scene.name) && preloadedSwaps[scene.name].Count > 0)
        {
            foreach (KeyValuePair<LocationData, GameObject> kvp in preloadedSwaps[scene.name].Where(kvp => kvp.Key.locationType == LocationType.Other))
            {
                Logger.Log(kvp.Key.itemChangerName);
                TrySwap(scene.name, kvp.Key, kvp.Value);
            }
        }
        else
        {
            return;
        }
    }

    private bool TrySwap(string sceneName, LocationData locationData, GameObject newModel)
    {
        try
        {
            if (GameSave.currentSave.IsKeyUnlocked($"AP_PickedUp-{locationData.itemChangerName}"))
            {
                // If we've already picked up this item, don't swap it
                return true;
            }
            GameObject newObject = SwapModel(sceneName, locationData, newModel);
            if (newObject.name.Contains("AP") || newObject.name.Contains("Soul"))
            {
                ItemFloater itemFloater = newObject.GetComponent<ItemFloater>();
                itemFloater.ySine = Mathf.PI / 2f;
                itemFloater.ySpeed = 0f;
                if (locationData.itemChangerName.Contains("Silent Servant"))
                {
                    itemFloater.yDistance = 10f;
                }
                else if (locationData.itemChangerName.Contains("Shrine"))
                {
                    itemFloater.yDistance = 3f;
                }
                else if (boostAmounts.ContainsKey(locationData.itemChangerName))
                {
                    itemFloater.yDistance = boostAmounts[locationData.itemChangerName];
                }
                else
                {
                    itemFloater.yDistance = 1f;
                }
            }
            else
            {
                float boostAmount = 0f;
                if (locationData.itemChangerName.Contains("Silent Servant"))
                {
                    boostAmount = 10f;
                }
                else if (locationData.itemChangerName.Contains("Shrine"))
                {
                    boostAmount = 3f;
                }
                else if (boostAmounts.ContainsKey(locationData.itemChangerName))
                {
                    boostAmount = boostAmounts[locationData.itemChangerName];
                }
                else
                {
                    boostAmount = 1f;
                }
                newObject.transform.SetPositionAndRotation(newObject.transform.position + new Vector3(0f, boostAmount, 0f), newObject.transform.rotation);
            }
            return true;
        }
        catch (NullReferenceException)
        {
            return false;
        }
        catch (ObjectInactiveException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private bool TrySwap(LocationData locationData, GameObject oldModel, GameObject newModel)
    {
        try
        {
            if (GameSave.currentSave.IsKeyUnlocked($"AP_PickedUp-{locationData.itemChangerName}"))
            {
                // If we've already picked up this item, don't swap it
                return true;
            }
            if (!oldModel.activeInHierarchy)
                {
                    return false;
                }
            GameObject newObject = SwapModel(oldModel, newModel);
            if (newObject.name.Contains("AP") || newObject.name.Contains("Soul"))
            {
                ItemFloater itemFloater = newObject.GetComponent<ItemFloater>();
                itemFloater.ySine = Mathf.PI / 2f;
                itemFloater.ySpeed = 0f;
                if (locationData.itemChangerName.Contains("Silent Servant"))
                {
                    itemFloater.yDistance = 10f;
                }
                else if (locationData.itemChangerName.Contains("Shrine"))
                {
                    itemFloater.yDistance = 3f;
                }
                else if (boostAmounts.ContainsKey(locationData.itemChangerName))
                {
                    itemFloater.yDistance = boostAmounts[locationData.itemChangerName];
                }
                else
                {
                    itemFloater.yDistance = 1f;
                }
            }
            else
            {
                float boostAmount = 0f;
                if (locationData.itemChangerName.Contains("Silent Servant"))
                {
                    boostAmount = 10f;
                }
                else if (locationData.itemChangerName.Contains("Shrine"))
                {
                    boostAmount = 3f;
                }
                else if (boostAmounts.ContainsKey(locationData.itemChangerName))
                {
                    boostAmount = boostAmounts[locationData.itemChangerName];
                }
                newObject.transform.SetPositionAndRotation(newObject.transform.position + new Vector3(0f, boostAmount, 0f), newObject.transform.rotation);
            }
            return true;
        }
        catch (NullReferenceException)
        {
            return false;
        }
        catch (ObjectInactiveException)
        {
            return false;
        }
    }

    internal class ObjectInactiveException : Exception
    {
        internal ObjectInactiveException()
        { }
        internal ObjectInactiveException(string message) : base(message)
        { }
        public ObjectInactiveException(string message, Exception innerException)
        : base(message, innerException)
        { }
    }

    private bool ConvertOtherGameNamesToDeathsDoor(string itemName, out string DDItemName)
    {
        if (itemName.IndexOf("key", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // If it has the substring key, regardless of if it is in the middle of the word, replace the model with a random key
            DDItemName = UnityEngine.Random.Range(0, 5) switch
            {
                0 => "Pink Key",
                1 => "Green Key",
                2 => "Yellow Key",
                3 => "Rusty Belltower Key",
                4 => "Makeshift Soul Key",
                _ => "Pink Key"
            };
            return true;
        }
        else if (itemName.IndexOf("soul", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("money", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "100 Souls";
            return true;
        }
        else if (itemName.IndexOf("seed", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("potion", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Life Seed";
            return true;
        }
        else if (itemName.IndexOf("mask", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Grunt's Old Mask";
            return true;
        }
        else if (itemName.IndexOf("seed", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("grass", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Life Seed";
            return true;
        }
        else if (itemName.IndexOf("ring", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Engagement Ring";
            return true;
        }
        else if (itemName.IndexOf("life", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Token of Death";
            return true;
        }
        else if (itemName.IndexOf("ladder", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Lever";
            return true;
        }
        else if (itemName.IndexOf("flower", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Undying Blossom";
            return true;
        }
        else if (itemName.IndexOf("crown", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("hat", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Ancient Crown";
            return true;
        }
        else if (itemName.IndexOf("music", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("song", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Surveillance Device";
            return true;
        }
        else if (itemName.IndexOf("relic", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("artifact", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("ancient", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("vintage", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Ancient Door Scale Model";
            return true;
        }
        else if (itemName.IndexOf("sword", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Reaper's Greatsword";
            return true;
        }
        else if (itemName.IndexOf("stick", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Discarded Umbrella";
            return true;
        }
        else if (itemName.IndexOf("bomb", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("firecracker", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Bomb";
            return true;
        }
        else if (itemName.IndexOf("arrow", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("bow", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("gun", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Arrow";
            return true;
        }
        else if (itemName.IndexOf("hook", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("grapple", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("Magic Orb", StringComparison.OrdinalIgnoreCase) >= 0 || itemName.IndexOf("clawshot", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Hookshot";
            return true;
        }
        else if (itemName.IndexOf("fire", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Fire";
            return true;
        }
        else if (itemName.IndexOf("page", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Death's Contract";
            return true;
        }
        else if (itemName.IndexOf("coin", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Shiny Medallion";
            return true;
        }
        else if (itemName.IndexOf("questagon", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            DDItemName = "Crow";
            return true;
        }
        DDItemName = "";
        return false;
    }

    private GameObject BaseModel(string newModel)
    {
        return Preloader.GetCachedObject<GameObject>(newModel);
    }
    private GameObject TrapModel()
    {
        Array values = Enum.GetValues(typeof(ItemRandomizer.ClassEnum));
        return RecolorSouls((ItemRandomizer.ClassEnum)values.GetValue(UnityEngine.Random.Range(0, values.Length)));
    }

    private GameObject RecolorSouls(ItemRandomizer.ClassEnum itemClassification)
    {
        if (Preloader.TryGetCachedObject<GameObject>($"AP {itemClassification}", out UnityEngine.Object obj))
        {
            return (GameObject)obj;
        }
        else
        {
            // Cache the recolored object if possible
            GameObject gameObject = Preloader.CacheAndReturnObject(Preloader.GetCachedObject<GameObject>("100 Souls"));
            gameObject.name = $"AP {itemClassification}";
            AMC.Models.Color color = itemClassification switch
            {
                ItemRandomizer.ClassEnum.Filler => AMC.Colors.BuiltInPalettes.Dark[AMC.Colors.ColorUtils.GetColor(AMC.Enums.ItemFlags.None)],
                ItemRandomizer.ClassEnum.ProgressionUseful => AMC.Models.Color.Yellow,
                ItemRandomizer.ClassEnum.Progression => AMC.Colors.BuiltInPalettes.Dark[AMC.Colors.ColorUtils.GetColor(AMC.Enums.ItemFlags.Advancement)],
                ItemRandomizer.ClassEnum.Useful => AMC.Colors.BuiltInPalettes.Dark[AMC.Colors.ColorUtils.GetColor(AMC.Enums.ItemFlags.NeverExclude)],
                _ => AMC.Models.Color.Black,
            };
            gameObject.GetComponent<MeshRenderer>().material.color = new Color(color.R / 255f, color.G / 255f, color.B / 255f, 1f);
            ParticleSystem.MainModule psmain = gameObject.GetComponentInChildren<ParticleSystem>().main;
            psmain.startColor = new Color(color.R * 1.3f / 255f, color.G * 1.3f / 255f, color.B * 1.3f / 255f, 1f);
            return gameObject;
        }
    }
    private GameObject SwapModel(string sceneName, LocationData locationData, GameObject newModel)
    {
        GameObject originalModel;
        if (locationData.locationType == LocationType.Other)
        {
            originalModel = PathUtil.GetByPath(sceneName, locationData.path);
        }
        else
        {
            originalModel = GetObjectForLocation(sceneName, locationData.itemChangerName, locationData.locationType);
        }
        if (!originalModel.activeSelf)
        {
            throw new ObjectInactiveException();
        }
        GameObject newObject = Instantiate(newModel, originalModel.transform.GetParent());
        originalModel.SetActive(false);
        return newObject;
    }

    private GameObject SwapModel(GameObject originalModel, GameObject newModel)
    {
        if (!originalModel.activeSelf)
        {
            throw new ObjectInactiveException();
        }
        GameObject newObject = Instantiate(newModel, originalModel.transform.GetParent());
        originalModel.SetActive(false);
        return newObject;
    }

    private GameObject GetObjectForLocation(string sceneName, string itemChangerName, LocationType locationType)
    {
        Logger.Log(itemChangerName);
        return locationType switch
        {
            LocationType.Shrine => ComponentUtil.FindAllComponentsOfTypeInScene<BaseKey>(sceneName).First(shrine => shrine.uniqueId == IC.Predefined.predefinedLocations[itemChangerName].UniqueId).transform.GetComponentInChildren<LightDistanceControl>().gameObject,
            LocationType.DropItem => ComponentUtil.FindAllComponentsOfTypeInScene<DropItem>(sceneName).First(drop => drop.uniqueId == IC.Predefined.predefinedLocations[itemChangerName].UniqueId).transform.Find("Visuals").gameObject,
            LocationType.Crow => ComponentUtil.FindAllComponentsOfTypeInScene<SoulKey>(sceneName).First(soulKey => soulKey.GetComponentInChildren<NPCCharacter>().speech_id[0].unlocks == IC.Predefined.predefinedLocations[itemChangerName].UniqueId).transform.Find("Soul/crow_2").gameObject,
            LocationType.Key => ComponentUtil.FindAllComponentsOfTypeInScene<CollectableKey>(sceneName).First(key => key.keyId == IC.Predefined.predefinedLocations[itemChangerName].UniqueId).transform.Find("keyHover").gameObject,
            _ => throw new NotImplementedException(),
        };
    }

    private GameObject FindGameObjectForItem(string scene, ItemData itemData)
    {
        ItemType itemType = itemData.type;
        return itemType switch
        {
            ItemType.RedAncientTabletOfKnowledge => GetTabletFromTruthTabletSpawner(scene, itemData),
            ItemType.YellowAncientTabletOfKnowledge => GetTabletFromTruthTabletSpawner(scene, itemData),
            ItemType.PurpleAncientTabletOfKnowledge => GetTabletFromTruthTabletSpawner(scene, itemData),
            ItemType.PinkAncientTabletOfKnowledge => GetTabletFromTruthTabletSpawner(scene, itemData),
            ItemType.Lever => GetLever(scene, itemData),
            ItemType.Fire or ItemType.Bomb or ItemType.Hookshot or ItemType.Bow => GetSpell(scene, itemData),
            _ => GetGameObjectDirectly(scene, itemData),
        };

    }

    private GameObject GetTabletFromTruthTabletSpawner(string scene, ItemData itemData)
    {
        TruthTabletSpawner tabletSpawner = PathUtil.GetByPath(scene, itemData.path).GetComponent<TruthTabletSpawner>();
        return tabletSpawner.objectPrefabToSpawn.GetComponentInChildren<MeshRenderer>(true).gameObject;
    }

    private GameObject GetLever(string scene, ItemData itemData)
    {
        GameObject lever = PathUtil.GetByPath(scene, itemData.path);
        DestroyImmediate(lever.GetComponentInChildren<ButtonPromptArea>(true).gameObject);
        lever.transform.localPosition = new Vector3(0f, 0f, 0f);
        return lever;
    }
    private GameObject GetSpell(string scene, ItemData itemData)
    {
        GameObject spell = PathUtil.GetByPath(scene, itemData.path);
        spell.transform.localPosition = new Vector3(0f, 0f, 0f);
        return spell;
    }

    private GameObject GetGameObjectDirectly(string scene, ItemData itemData)
    {
        return PathUtil.GetByPath(scene, itemData.path);
    }

    internal class ItemData
    {
        internal ItemType type;
        internal string name;
        internal string path;
    }

    internal class LocationData
    {
        internal string itemChangerName;
        internal string path = "";
        internal LocationType locationType = LocationType.Other;
    }

    private void TryInitialSwap(string objectSceneName, string id, GameObject instanceGameObject, Func<GameObject, GameObject> getCorrectChild)
    {
        if (!Archipelago.Instance.apConfig.ModelSwapper || Preloader.IsPreloading)
        {
            return;
        }
        if (!Instance.preloadedSwaps.Keys.Contains(objectSceneName))
        {
            return;
        }
        if (Instance.preloadedSwaps[objectSceneName].Any(kvp => IC.Predefined.predefinedLocations[kvp.Key.itemChangerName].UniqueId == id))
        {
            KeyValuePair<LocationData, GameObject> kvp = Instance.preloadedSwaps[objectSceneName].First(kvp => IC.Predefined.predefinedLocations[kvp.Key.itemChangerName].UniqueId == id);
            try
            {
                Instance.TrySwap(kvp.Key, getCorrectChild(instanceGameObject), kvp.Value);
            }
            catch (NullReferenceException)
            {
                return;
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }
    }

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemFloater), nameof(ItemFloater.Start))]
        private static bool PreItemFloaterStart()
        {
            if (Archipelago.Instance.apConfig.ModelSwapper)
            {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BaseKey), nameof(BaseKey.Awake))]
        private static void PostBaseKeyAwake(BaseKey __instance)
        {
			static GameObject getCorrectChild(GameObject instanceGameObject) => instanceGameObject.transform.GetComponentInChildren<LightDistanceControl>().gameObject;
			Instance.TryInitialSwap(__instance.gameObject.scene.name, __instance.uniqueId, __instance.gameObject, getCorrectChild);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DropItem), nameof(DropItem.Awake))]
        private static void PostDropItemAwake(DropItem __instance)
        {
            Func<GameObject, GameObject> getCorrectChild;

            if (__instance.uniqueId == "hammer" || __instance.uniqueId == "sword_heavy")
            {
                getCorrectChild = instanceGameObject => instanceGameObject.transform.Find("GameObject/Visuals").gameObject;
            }
            else
            {
                getCorrectChild = instanceGameObject => instanceGameObject.transform.Find("Visuals").gameObject;
            }
            Instance.TryInitialSwap(__instance.gameObject.scene.name, __instance.uniqueId, __instance.gameObject, getCorrectChild);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SoulKey), nameof(SoulKey.Start))]
        private static void PostSoulKeyStart(SoulKey __instance)
        {
			static GameObject getCorrectChild(GameObject instanceGameObject) => instanceGameObject.transform.Find("Soul/crow_2").gameObject;
			Instance.TryInitialSwap(__instance.gameObject.scene.name, __instance.GetComponentInChildren<NPCCharacter>().speech_id[0].unlocks.Replace("ItemChanger-collected_crow_location_", ""), __instance.gameObject, getCorrectChild);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(CollectableKey), nameof(CollectableKey.Start))]
        private static void PostCollectableKeyStart(CollectableKey __instance)
        {
            static GameObject getCorrectChild(GameObject instanceGameObject) => instanceGameObject.transform.Find("keyHover").gameObject;
			Instance.TryInitialSwap(__instance.gameObject.scene.name, __instance.keyId, __instance.gameObject, getCorrectChild);
        }
    }
}
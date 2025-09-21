using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace DDoor.ArchipelagoRandomizer;

internal class EnemyRandomizer : MonoBehaviour
{
	public enum EnemyType
	{
		BatBlack,
		BatWhite,
		Brute,
		BruteGold,
		DekuScrub,
		Dodger,
		Fireplant,
		FireplantNoHookshot,
		Ghoul,
		GhoulLongRange,
		GhoulRapid,
		GrimaceKnight,
		Grunt,
		GruntFrog,
		GruntGrave,
		GruntPot,
		GruntRedeemer,
		GruntRedeemerIndoor,
		GruntYeti,
		Headroller,
		HeadrollerHeadless,
		Jumper,
		Knight,
		KnightGreenPlague,
		Lurker,
		LurkerMother,
		Mage,
		MageFire,
		MageGrounded,
		MagePlague,
		MagePlagueSingleshot,
		MageRedPurple,
		PlagueKnight,
		PlagueKnightSlowFire,
		Plant,
		PotMimicAvariceExplode,
		PotMimicAvariceMagic,
		PotMimicAvariceMelee,
		PotMimicExplode,
		PotMimicMagic,
		PotMimicMelee,
		ServantAll,
		ServantBomb,
		ServantFire,
		ServantHookshot,
		SlimeBig,
		SlimeBigGreen,
		SlimeMed,
		SlimeMedGreen,
		SlimeSmall,
		SlimeSmallGreen,
	}

	private static EnemyRandomizer instance;
	private static readonly Array enemyValues = Enum.GetValues(typeof(EnemyType));
	private Dictionary<string, EnemyData[]> enemiesToCache;
	private Dictionary<EnemyType, EnemyData> enemyTypeLookup = new();
	private List<string> enemiesToNotReplace;

	public static EnemyRandomizer Instance => instance;

	private void Awake()
	{
		instance = this;
		enemiesToCache = new()
		{
			{"AVARICE_WAVES_Secret", [
				new EnemyData { type = EnemyType.Lurker, name = "_E_LURKER" },
				new EnemyData { type = EnemyType.Knight, name = "_E_KNIGHT" },
				new EnemyData { type = EnemyType.MageRedPurple, name = "_E_MAGE_REDPURPLE Variant" },
				new EnemyData { type = EnemyType.BruteGold, name = "_E_BRUTE_GOLD Variant" },
				new EnemyData { type = EnemyType.GruntRedeemerIndoor, name = "_E_GRUNT_Redeemer_Indoor Variant" },
				new EnemyData { type = EnemyType.MagePlagueSingleshot, name = "_E_MAGE_Plague_SingleShot" },
				new EnemyData { type = EnemyType.BatWhite, name = "_E_BAT_White" },
				new EnemyData { type = EnemyType.GhoulRapid, name = "_E_GHOUL_Rapid Variant" },
				new EnemyData { type = EnemyType.LurkerMother, name = "_E_LURKER_MOTHER" },
			]},
			{"lvl_Forest", [
				new EnemyData { type = EnemyType.DekuScrub, name = "_E_DEKU_SCRUB" },
				new EnemyData { type = EnemyType.Plant, name = "_E_PLANT" },
			]},
			{"lvl_Graveyard", [
				new EnemyData { type = EnemyType.GruntRedeemer, name = "_E_GRUNT_Redeemer" },
				new EnemyData { type = EnemyType.GruntGrave, name = "_E_GRUNT_Grave Variant" },
				new EnemyData { type = EnemyType.BatBlack, name = "_E_BAT_Black Variant" },
				new EnemyData { type = EnemyType.HeadrollerHeadless, name = "_E_HEADROLLER_HEADLESS Variant" },
			]},
			{"lvl_GrandmaGardens", [
				new EnemyData { type = EnemyType.SlimeMedGreen, name = "_E_Slime_med_GREEN Variant" },
				new EnemyData { type = EnemyType.SlimeSmallGreen, name = "_E_Slime_small_GREEN Variant" },
				new EnemyData { type = EnemyType.SlimeBigGreen, name = "_E_Slime_big_GREEN Variant" },
				new EnemyData { type = EnemyType.PotMimicAvariceExplode, name = "POT_Mimic_Explode_AVARICE Variant" },
				new EnemyData { type = EnemyType.PotMimicAvariceMagic, name = "POT_Mimic_Magic_AVARICE Variant" },
				new EnemyData { type = EnemyType.PotMimicAvariceMelee, name = "POT_Mimic_Melee_AVARICE Variant" },
			]},
			{"lvl_GrandmaMansion", [
				new EnemyData { type = EnemyType.PotMimicMagic, name = "POT_Mimic_Magic" },
				new EnemyData { type = EnemyType.PotMimicExplode, name = "POT_Mimic_Explode" },
				new EnemyData { type = EnemyType.PotMimicMelee, name = "POT_Mimic_Melee" },
			]},
			{"lvl_Swamp", [
				new EnemyData { type = EnemyType.FireplantNoHookshot, name = "_E_FIREPLANT_Nohookshot" },
				new EnemyData { type = EnemyType.GhoulLongRange, name = "_E_GHOUL_LongRange Variant" },
				new EnemyData { type = EnemyType.Grunt, name = "_E_GRUNT" },
				new EnemyData { type = EnemyType.MagePlague, name = "_E_MAGE_Plague Variant" },
			]},
			{"lvl_mountaintops", [
				new EnemyData { type = EnemyType.Brute, name = "_E_BRUTE" },
				new EnemyData { type = EnemyType.Fireplant, name = "_E_FIREPLANT" },
				new EnemyData { type = EnemyType.MageGrounded, name = "_E_MAGE_Grounded Variant" },
			]},
			{"AVARICE_WAVES_Fortress", [
				new EnemyData { type = EnemyType.SlimeSmall, name = "_E_Slime_small" },
				new EnemyData { type = EnemyType.Jumper, name = "_E_JUMPER" },
				new EnemyData { type = EnemyType.SlimeMed, name = "_E_Slime_med" },
				new EnemyData { type = EnemyType.MageFire, name = "_E_MAGE_Fire Variant" },
				new EnemyData { type = EnemyType.GruntYeti, name = "_E_GRUNT_Yeti Variant" },
			]},
			{"lvl_SailorMountain", [
				new EnemyData { type = EnemyType.PlagueKnightSlowFire, name = "_E_PLAGUE_KNIGHT_SlowFire" },
			]},
			{"lvl_GrandmaBasement", [
				new EnemyData { type = EnemyType.SlimeBig, name = "_E_Slime_big" },
				new EnemyData { type = EnemyType.Headroller, name = "_E_HEADROLLER" },
				new EnemyData { type = EnemyType.PlagueKnight, name = "_E_PLAGUE_KNIGHT" },
			]},
			{"TEST_AREA", [
				new EnemyData { type = EnemyType.Mage, name = "_E_MAGE" },
				new EnemyData { type = EnemyType.GruntPot, name = "_E_GRUNT_Pot Variant" },
				new EnemyData { type = EnemyType.GrimaceKnight, name = "_E_GRIMACE_KNIGHT" },
				new EnemyData { type = EnemyType.Dodger, name = "_E_DODGER" },
				new EnemyData { type = EnemyType.KnightGreenPlague, name = "_E_KNIGHT_GreenPlague" },
				new EnemyData { type = EnemyType.Ghoul, name = "_E_GHOUL" },
			]},
			{"lvl_SilentServant_Fight", [
				new EnemyData { type = EnemyType.ServantAll, name = "_E_SERVANT_All" },
				new EnemyData { type = EnemyType.ServantBomb, name = "_E_SERVANT_Bombs" },
				new EnemyData { type = EnemyType.ServantFire, name = "_E_SERVANT_FIRE Variant" },
				new EnemyData { type = EnemyType.ServantHookshot, name = "_E_SERVANT_Hookshot Variant" },
			]},
			{"boss_Frog", [
				new EnemyData { type = EnemyType.GruntFrog, name = "_E_GRUNT_Frog" },
			]},
		};
		enemiesToNotReplace = new()
		{
			"redeemer_BOSS",
			"redeemer_cutscene",
			"BOSS_GraveDigger",
			"_FORESTMOTHER_BOSS",
			"BOSS_lord_of_doors NEW",
			"BOSS_lord_of_doors_Betty",
			"BOSS_lord_of_doors_Garden",
			"lord_of_doors",
			"lord_of_doorsOLD",
			"BOSS_lord_of_doors_Forest",
			"grandma",
			"oldfrog",
			"FROG_BOSS_FAT",
			"FROG_BOSS_MAIN",
			"betty_boss",
		};

		// Setup enemyType -> EnemyData lookup dict
		foreach (EnemyData[] dataArr in enemiesToCache.Values)
		{
			foreach (EnemyData data in dataArr)
			{
				if (!enemyTypeLookup.ContainsKey(data.type))
					enemyTypeLookup.Add(data.type, data);
			}
		}
	}

	private void OnEnable()
	{
		Logger.Log("EnemyType randomizer started!");

		// Preload enemies
		foreach (var kvp in enemiesToCache)
		{
			string sceneName = kvp.Key;
			EnemyData[] dataForEnemies = kvp.Value;

			Preloader.Instance.AddObjectToCacheList(sceneName, () =>
			{
				List<GameObject> enemies = new();

				foreach (EnemyData enemyData in dataForEnemies)
				{
					GameObject foundEnemy = null;

					foreach (AI_Brain brain in Resources.FindObjectsOfTypeAll<AI_Brain>())
					{
						if (brain.name == enemyData.name)
						{
							foundEnemy = brain.gameObject;
							foundEnemy.SetActive(true);

							// Modify silent servants
							if (foundEnemy.TryGetComponent(out AI_SilentServant ai))
							{
								DestroyImmediate(ai.GetComponentInChildren<CameraFocusObjectLimited>(true).gameObject);
								ai.hasHookshot = false;

								if (enemyData.type == EnemyType.ServantAll)
								{
									ai.hasAllPowers = false;
									ai.hasBombs = true;
									ai.hasFire = true;
								}
							}
						}
					}

					if (foundEnemy == null)
					{
						Logger.LogError($"During enemy rando preload, failed to find enemy\n    {enemyData.name}\n    in {sceneName}");
						continue;
					}

					enemies.Add(foundEnemy);
				}

				Logger.Log($"Finished caching enemies for scene {sceneName}!");
				return enemies.ToArray();
			});
		}

		Preloader.Instance.OnPreloadDone += AfterPreload;
	}

	private void OnDisable()
	{
		Preloader.Instance.OnPreloadDone -= AfterPreload;
		SceneManager.sceneLoaded -= SceneLoaded;
	}

	//TODO: Remove this, as this is for quick testing before figuring out enemy replacements

	//int index = 0;
	//private void Update()
	//{
	//	if (Input.GetKeyDown("t"))
	//	{
	//		SpawnEnemyAtPlayer((EnemyType)index);
	//		index++;
	//	}
	//}

	private void AfterPreload()
	{
		SceneManager.sceneLoaded += SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode _)
	{
		ReplaceEnemiesInScene();
	}

	public GameObject SpawnEnemyAtPlayer(EnemyType enemyType)
	{
		Transform player = PlayerGlobal.instance.transform;
		Vector3 position = player.position - player.forward * 2f;
		GameObject enemy = Instantiate(GetEnemy(enemyType));
		enemy.transform.position = position;
		return enemy;
	}

	private void ReplaceEnemiesInScene()
	{
		Stopwatch sw = Stopwatch.StartNew();
		Scene scene = SceneManager.GetActiveScene();

		foreach (GameObject rootObj in scene.GetRootGameObjects())
		{
			foreach (Component comp in rootObj.GetComponentsInChildren<Component>(true))
			{
				GameObject replacement = null;

				switch (comp)
				{
					// Replace enemies that don't spawn from spawners
					case AI_Brain brain:
						// Don't replace silent servants because there's no good way to trigger
						// the death cutscene afterwards, so player gets stuck and gets no item
						if (brain is AI_SilentServant)
							break;

						replacement = GetEnemyReplalcement(comp);

						if (replacement == null)
							break;

						Vector3 enemyPos = brain.transform.position;
						Transform enemyParent = brain.transform.parent;
						Destroy(brain.gameObject);
						GameObject newEnemy = Instantiate(replacement, enemyParent);
						newEnemy.transform.position = enemyPos;
						break;
					// Replace enemies that spawn from spawners
					case WaveEnemy wave:
						replacement = GetEnemyReplalcement(comp);

						if (replacement == null)
							break;

						wave.prefab = replacement;
						break;
					// Replace enemies that spawn from eggs (Lurkers)
					case LurkerEgg egg:
						replacement = GetEnemyReplalcement(comp);

						if (replacement == null)
							break;

						egg.contentsPrefab = replacement;
						break;
				}
			}
		}

		sw.Stop();
		Logger.Log($"Took {sw.ElapsedMilliseconds}ms to replace all enemies in {scene.name}");
	}

	private GameObject GetEnemyReplalcement(Component comp)
	{
		string enemyName = comp switch
		{
			AI_Brain brain => brain.name,
			WaveEnemy wave => wave.prefab.name,
			LurkerEgg egg => egg.contentsPrefab.name,
			_ => ""
		};

		// Ignore exclusions
		if (enemiesToNotReplace.Contains(enemyName))
			return null;

		GameObject randomEnemy = GetRandomEnemy();
		return randomEnemy;
	}

	private GameObject GetRandomEnemy()
	{
		EnemyType[] types = enemyTypeLookup.Keys.ToArray();
		int randomIndex = Random.Range(0, types.Length);
		EnemyType randomEnemy = types[randomIndex];
		return GetEnemy(randomEnemy);
	}

	private GameObject GetEnemy(EnemyType enemyType)
	{
		if (enemyTypeLookup.TryGetValue(enemyType, out EnemyData data))
		{
			return Preloader.GetCachedObject<GameObject>(data.name);
		}

		Logger.LogError($"No enemy {enemyType} was found in cache list");
		return null;
	}

	private IEnumerator SilentServantDeath(AI_SilentServant servant)
	{
		yield return new WaitForSeconds(3);
		Destroy(servant.gameObject);
	}

	private struct EnemyData
	{
		public EnemyType type;
		public string name;
	}

	[HarmonyPatch]
	private class Patches
	{
		// Removes the corpse of killed silent servants, as the cutscene that removes them doesn't play
		[HarmonyPostfix]
		[HarmonyPatch(typeof(AI_SilentServant), nameof(AI_SilentServant.OnDeath))]
		private static void SilentServantDeathRemoveCorpsePatch(AI_SilentServant __instance)
		{
			if (!Archipelago.Instance.IsConnected())
				return;

			Instance.StartCoroutine(Instance.SilentServantDeath(__instance));
		}

		// Spawns the randomized enemy whewn a Lurker (spider) egg explodes
		[HarmonyPrefix]
		[HarmonyPatch(typeof(LurkerEgg), nameof(LurkerEgg.Explode))]
		private static bool LurkerEggExplodePatch(LurkerEgg __instance, Vector3 source, float modifier = 1f)
		{
			if (!__instance.didExplode)
			{
				__instance.didExplode = true;
				Vector3 vector = __instance.transform.position - source;
				vector.Normalize();
				vector.x += Random.Range(-0.1f, 0.1f);
				vector.z += Random.Range(-0.1f, 0.1f);
				float y = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
				GameObject gameObject = Instantiate(__instance.contentsPrefab, __instance.transform.position, Quaternion.Euler(0f, y, 0f));
				gameObject.transform.SetParent(GameRoom.GetCurrentContents());
				Destroy(__instance.gameObject);
				return false;
			}

			return true;
		}
	}
}
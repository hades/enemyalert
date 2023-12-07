using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace enemyalert
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Lethal Company.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            Logger.LogInfo($"enemyalert v. {PluginInfo.PLUGIN_VERSION}");
            Logger.LogInfo($"Shows a message whenever a new enemy is spawned outside the ship");
            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll(Assembly.GetExecutingAssembly());
        }

        internal static void NotifyAboutEnemy(ref GameObject enemy)
        {
            string name = enemy.GetComponent<EnemyAI>().enemyType.enemyName;
            string time = $"{TimeOfDay.Instance.normalizedTimeOfDay:p}";
            HUDManager.Instance.AddTextToChatOnServer(
                $"<i><color=#ff2400><size=75%>{time}: spawned {name}</size></color></i>");
        }
    }

    [HarmonyPatch(typeof(RoundManager))]
    internal class RoundManagerPatch
    {
        [HarmonyPatch("SpawnRandomOutsideEnemy")]
        [HarmonyPostfix]
        public static void RandomOutsideEnemySpawned(ref GameObject __result)
        {
            Plugin.NotifyAboutEnemy(ref __result);
        }

        [HarmonyPatch("SpawnRandomDaytimeEnemy")]
        [HarmonyPostfix]
        public static void RandomDaytimeEnemySpawned(ref GameObject __result)
        {
            Plugin.NotifyAboutEnemy(ref __result);
        }
    }
}

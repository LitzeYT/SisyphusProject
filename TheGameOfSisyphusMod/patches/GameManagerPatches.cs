using System.Threading;
using BepInEx.Logging;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;
using Logger = BepInEx.Logging.Logger;

namespace TheGameOfSisyphusMod.patches
{
    [HarmonyPatch(typeof(GameManager))]
    public class GameManagerPatches
    {
        private static GameObject _rock;
        
        [HarmonyPatch("GenerateBall")]
        [HarmonyPostfix]
        public static void GenerateBall_postfix()
        {
            //log the ball generation
            ManualLogSource logger = new ManualLogSource("TheGameOfSisyphusMod");
            Logger.Sources.Add(logger);
            
            logger.LogInfo("Ball generated");
            
            //find Rock(Clone) and log its position
            GameObject rock = GameObject.Find("Rock(Clone)");
            if (rock != null && !_rock)
            {
                _rock = rock;
            }
        }
        
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update_postfix()
        {
            //log the update
            ManualLogSource logger = new ManualLogSource("TheGameOfSisyphusMod");
            Logger.Sources.Add(logger);

            if (_rock)
            {
                logger.LogInfo($"Rock position: {_rock.transform.position}");
            }
        }
    }
}
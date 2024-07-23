using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Logger = BepInEx.Logging.Logger;

namespace TheGameOfSisyphusMod.patches
{
    [HarmonyPatch(typeof(GameManager))]
    public class GameManagerPatches
    {
        private static GameObject _rock;
        private static ManualLogSource logger;
        private static HttpClient client;
        private static string userId = "hardcodedUserID"; // Hardcoded UserID
        private static float lastUpdate;
        private static bool initialized;
        private static float updateInterval = 2.0f;

        [HarmonyPatch("GenerateBall")]
        [HarmonyPostfix]
        public static void GenerateBall_postfix()
        {
            // Log the ball generation
            if (!initialized)
            {
                logger = new ManualLogSource("TheGameOfSisyphusMod");
                Logger.Sources.Add(logger);
                client = new HttpClient();
                initialized = true;
            }

            logger.LogInfo("Ball generated");

            // Find Rock(Clone) and log its position
            GameObject rock = GameObject.Find("EmeraldRock(Clone)");

            if(rock != null)
            {
                logger.LogInfo($"Rock position: {rock.transform.position.y}");
            }



            if (rock != null && _rock == null)
            {
                _rock = rock;
                Task.Run(() => NotifyBallGenerated());
            }
        }

        private static async Task NotifyBallGenerated()
        {
            logger.LogInfo("Notifying server of ball generation.");
            var response = await client.PostAsync($"http://91.107.224.122:5001/api/v1/User/{userId}/{_rock.transform.position.y}", new StringContent(""));
            if (response.IsSuccessStatusCode)
            {
                logger.LogInfo("Server notified of ball generation.");
            }
            else
            {
                logger.LogError("Failed to notify server of ball generation.");
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update_postfix()
        {
            if (Time.time - lastUpdate >= updateInterval && _rock != null)
            {
                lastUpdate = Time.time;
                Task.Run(() => UpdateBallPosition());
            }
        }

        private static async Task UpdateBallPosition()
        {
            var response = await client.PostAsync($"http://91.107.224.122:5001/api/v1/User/{userId}/{_rock.transform.position.y}", new StringContent(""));
            if (response.IsSuccessStatusCode)
            {
                logger.LogInfo($"Rock position updated: {_rock.transform.position.y}");
            }
            else
            {
                logger.LogError("Failed to update rock position.");
            }
        }
    }
}

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Newtonsoft.Json;
using System.Numerics;

namespace TheGameOfSisyphusMod.patches
{
    [HarmonyPatch(typeof(GameManager))]
    public class GameManagerPatches
    {
        private static GameObject _emeraldRock;
        private static readonly HttpClient client = new HttpClient();

        [HarmonyPatch("GenerateBall")]
        [HarmonyPostfix]
        public static void GenerateBall_postfix()
        {
            //log the ball generation
            ManualLogSource logger = new ManualLogSource("TheGameOfSisyphusMod");
            BepInEx.Logging.Logger.Sources.Add(logger);

            logger.LogInfo("Ball generated");

            //find EmeraldRock(Clone) and log its position
            GameObject emeraldRock = GameObject.Find("EmeraldRock(Clone)");
            if (emeraldRock != null && _emeraldRock == null)
            {
                _emeraldRock = emeraldRock;
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static async void Update_postfix()
        {
            //log the update
            ManualLogSource logger = new ManualLogSource("TheGameOfSisyphusMod");
            BepInEx.Logging.Logger.Sources.Add(logger);

            if (_emeraldRock)
            {
                UnityEngine.Vector3 emeraldRockPosition = _emeraldRock.transform.position;
                logger.LogInfo($"Emerald Rock position: {emeraldRockPosition}");

                // Replace "your-username" with the actual username you want to send
                string username = "your-username";
                await SendRockPositionToServer(username, emeraldRockPosition);
            }
        }

        private static async Task SendRockPositionToServer(string userId, UnityEngine.Vector3 position)
        {
            string url = "http://91.107.224.122:5001/api/v1/User";
            var data = new
            {
                userid = userId,
                points = position.ToString()
            };

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            
            //log the update
            ManualLogSource logger = new ManualLogSource("TheGameOfSisyphusMod");
            BepInEx.Logging.Logger.Sources.Add(logger);
            
            try
            {
                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    logger.LogInfo("Data successfully sent to server");
                }
                else
                {
                    logger.LogError($"Failed to send data to server. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                logger.LogError($"Request error: {e.Message}");
            }
        }
    }
}

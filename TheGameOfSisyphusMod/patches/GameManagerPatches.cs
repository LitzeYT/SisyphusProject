using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;


namespace TheGameOfSisyphusMod.patches
{
    [HarmonyPatch(typeof(GameManager))]
    public class GameManagerPatches : MonoBehaviour
    {
        private static GameObject _emeraldRock;
        private const string Url = "https://hobauflock.cloud/api/v1/User/";
        private const float Interval = 1f; // Interval in seconds
        private static ManualLogSource _logger;
        private static string UserId = "";

        static GameManagerPatches()
        {
            _logger = new ManualLogSource("TheGameOfSisyphusMod");
            BepInEx.Logging.Logger.Sources.Add(_logger);
        }

        [HarmonyPatch("GenerateBall")]
        [HarmonyPostfix]
        public static void GenerateBall_postfix()
        {
            _logger.LogInfo("Ball generated");

            if (_emeraldRock == null)
            {
                _emeraldRock = GameObject.Find("EmeraldRock(Clone)");
            }

            _logger.LogInfo("EmeraldRock: " + _emeraldRock);

            string path = Application.dataPath + "/ModdingConfig/ModdingConfig.json";
            if (System.IO.File.Exists(path))
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                UserId = lines[0];   

                _logger.LogInfo("Loaded UserId: " + UserId);
                //load texture form ModdingConfig folder "Rock_emerald_AlbedoTransparency.png" and apply it to the rock material
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);                
                texture.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + "/ModdingConfig/Rock_emerald_AlbedoTransparency.png"));                
                texture.wrapMode = TextureWrapMode.Clamp;
                var material = _emeraldRock.GetComponent<MeshRenderer>().material;                
                material.mainTexture = texture;

                //load Rock_emerald_MetallicSmoothness.png and apply it to the rock materials metallic and smoothness
                var texture2 = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                texture2.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + "/ModdingConfig/Rock_emerald_MetallicSmoothness.png"));
                texture2.wrapMode = TextureWrapMode.Clamp;
                material.SetTexture("_MetallicGlossMap", texture2);
                
            }
            else
            {
                _logger.LogError("ModdingConfig.json not found");               
                System.IO.Directory.CreateDirectory(Application.dataPath + "/ModdingConfig");
                System.IO.File.WriteAllText(path, "Replace_this_with_your_user_id_no_quotes_no_spaces_atleast_3_characters");   

                //quit the game
                Application.Quit();            
            }


            _logger.LogInfo("UserId: " + UserId);



            if (_emeraldRock != null)
            {
                MonoBehaviourSingleton.Instance.StartCoroutine(SendRockPositionToServer(UserId, _emeraldRock));
            }


        }

        private static IEnumerator SendRockPositionToServer(string userId, GameObject stone)
        {
            while (true)
            {
                _logger.LogInfo("Sending data to server");

                int height = (int)(stone.transform.position.y * 100);
                string ls = Url + userId + "/" + height;
                _logger.LogInfo("URL: " + ls);
                using (UnityWebRequest request = new UnityWebRequest(ls, "POST"))
                {

                    request.downloadHandler = new DownloadHandlerBuffer();

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        _logger.LogInfo("Data sent to server");
                    }
                    else
                    {
                        _logger.LogError("Error sending data to server: " + request.error);
                    }
                }

                yield return new WaitForSeconds(Interval);
            }
        }

        [System.Serializable]
        public class PositionData
        {
            public string UserId;
            public Vector3Data Points;
        }

        [System.Serializable]
        public class Vector3Data
        {
            public float X;
            public float Y;
            public float Z;
        }
    }

    // Singleton class to provide access to StartCoroutine
    public class MonoBehaviourSingleton : MonoBehaviour
    {
        private static MonoBehaviourSingleton _instance;

        public static MonoBehaviourSingleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("MonoBehaviourSingleton");
                    _instance = go.AddComponent<MonoBehaviourSingleton>();
                }
                return _instance;
            }
        }
    }
}
using BepInEx;
using HarmonyLib;
using TheGameOfSisyphusMod.patches;
using UnityEngine;

namespace TheGameOfSisyphusMod
{
    [BepInPlugin("com.justin8303.thegameofsisyphusmod", "TheGameOfSisyphusMod", "1.0.0")]
    public class BepinxLoader : BaseUnityPlugin
    {
        public const string
            MODNAME = "TGOSMod",
            AUTHOR = "Justin8303",
            GUID = AUTHOR + "." + MODNAME,
            VERSION = "1.0.0";

        public static Harmony HarmonyInstance;
        
        public void Awake()
        {
            Logger.LogInfo($"Loaded {MODNAME} v{VERSION} by {AUTHOR}");
            
            HarmonyInstance = new Harmony(GUID);
            HarmonyInstance.PatchAll(typeof(GameManagerPatches));
            Logger.LogInfo("Patched GameManager.GenerateBall");
        }
    }
}
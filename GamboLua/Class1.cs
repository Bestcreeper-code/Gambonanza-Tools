using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using Blukulele.CHE;
using HarmonyLib;
using UnityEngine;

namespace Gambo
{
    
    [BepInPlugin("com.bestcreeper.gambolua", "GamboLua", "1.0.0")]
    public class PluginMain : BaseUnityPlugin
    {
        
        public static ManualLogSource _logger;
        
        public readonly Harmony _harmony = new Harmony("com.bestcreeper.gambolua");

        public void Awake()
        {
            Logger.LogInfo("Plugin is loaded!");

            GameObject updaterObject = new GameObject("PluginUpdater");
            updaterObject.AddComponent<PluginUpdater>();
            DontDestroyOnLoad(updaterObject);

            _logger = Logger;
            
            // GamboLuaManager.InitLua();
            GamboLuaManager.InitLua(Path.Combine(Paths.PluginPath, "GamboLua"));
            
            _harmony.PatchAll();
        }
        
        
       
        public static void UpdatePlugin()
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.O))
                {

                    {
                        GambitLib.testnewgambit();   
                        // Gambo_LuaFuncs.currentGambit = new GamboLuaGambit();


                        string testscript = File.ReadAllText(Path.Combine(Paths.PluginPath, "GamboLua", "test.lua"));
 

                        // execute
                        GamboLuaManager.LuaScript?.DoString(testscript);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e);
            }
        }
    }

    public class PluginUpdater : MonoBehaviour
    {
        void Update()
        {
            try
            {
                PluginMain.UpdatePlugin();
            }
            catch (Exception e)
            {
                PluginMain._logger.LogError($"Plugin updater error: {e}");
            }
        }
    }
}
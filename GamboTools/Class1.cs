using System;
using System.IO;
using BepInEx;
using BepInEx.Logging;
using Blukulele.CHE;
using Blukulele.Core;
using HarmonyLib;
using UnityEngine;

namespace Gambo
{
    
    [BepInPlugin("com.bestcreeper.gambotools", "GamboTools", "1.0.0")]
    public class PluginMain : BaseUnityPlugin
    {
        
        public static ManualLogSource _logger;
        public static PluginMain Instance { get; private set; }
        public static string PluginDIr { get; private set; }
        
        public readonly Harmony _harmony = new Harmony("com.bestcreeper.gambotools");

        public void Awake()
        {
            Logger.LogInfo("Plugin is loaded!");

            GameObject updaterObject = new GameObject("PluginUpdater");
            PluginUpdater.Instance = updaterObject.AddComponent<PluginUpdater>();
            DontDestroyOnLoad(updaterObject);
            
            PluginDIr = Path.GetDirectoryName(Info.Location);

            _logger = Logger;
            
            
            
            
            _harmony.PatchAll();
        }


        private static bool imgui_inited = false;
        public static void UpdatePlugin()
        {
            if (GambitLibrary.Instance && !imgui_inited)
            {
                UI.Init();
                imgui_inited = true;
                UI.show_window = true;
                
            }


            if (Input.GetKeyDown(KeyCode.F10))
            {
                UI.show_window = !UI.show_window;
            }
        }
    }

    public class PluginUpdater : MonoBehaviour
    {
        public static PluginUpdater Instance;
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

        private void OnGUI()
        {
            UI.Render();
        }
    }
}
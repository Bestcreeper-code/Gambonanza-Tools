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

        private static KeyCode ui_toggle_input = KeyCode.None;
        
        public readonly Harmony _harmony = new Harmony("com.bestcreeper.gambotools");
        

        public void Awake()
        {
            Logger.LogInfo("Plugin is loaded!");

            GameObject updaterObject = new GameObject("PluginUpdater");
            PluginUpdater.Instance = updaterObject.AddComponent<PluginUpdater>();
            DontDestroyOnLoad(updaterObject);
            
            PluginDIr = Path.GetDirectoryName(Info.Location);

            var lines = File.ReadAllLines(Path.Combine(PluginDIr, "config.cfg"));

            foreach (var line in lines)
            {
                if (line.StartsWith("Debug_Key:"))
                {

                    var value = line.Replace("Debug_Key:", "").Trim();

                    
                    

                    if (!Enum.TryParse<KeyCode>(value, true, out ui_toggle_input))
                    {
                        Logger.LogWarning($"Failed to parse key: [{value}]");
                        ui_toggle_input = KeyCode.None;
                    }
                    
                }
            }
            if (ui_toggle_input == KeyCode.None)
            {
                ui_toggle_input = KeyCode.F10;
            }
            
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


            if (Input.GetKeyDown(ui_toggle_input))
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
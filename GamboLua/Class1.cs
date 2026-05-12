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
                        // Gambo_LuaFuncs.currentGambit = new GamboLuaGambit();


                        string testscript = File.ReadAllText(Path.Combine(Paths.PluginPath, "GamboLua", "test.lua"));
                        
//                         string testscript = @"
//                             print('Lua executing Gambonanza.GiveGold(10)')
//
//                             local result = Gambonanza.GiveGold(10)
//                             print('return code' .. result)
//
//                             print('testing board interact')
//
//                             local v = Gambonanza.GetBoardSize()
//                             print('board width' .. v.x)
//                             print('board height' .. v.y)
//                             local t = Gambonanza.GetTile(0,0)
//                             print('tile name' .. t.name)
//
//                             print('adding gold king to stock')
//                             local still_has_space = Gambonanza.AddStockPiece(PieceType.KING,true)
//
//                             if still_has_space then
//                                 print('success')
//                             else
//                                 print('stock full')
//                             end
//                         ";

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
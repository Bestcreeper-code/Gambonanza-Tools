using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Blukulele.CHE;
using Blukulele.Core;
using MoonSharp.Interpreter;
using UnityEngine;


namespace Gambo
{
    public class GamboLuaManager
    {
        
        public static Script LuaScript = new Script();
        public static GameObject debug_object;
        
        public static void InitLua(string pluginPath)
        {
            LuaScript.Options.DebugPrint = s => PluginMain._logger.LogInfo("[Lua] " + s);
            
            RegisterTypesLua();


            LuaScript.Globals["Gambonanza"] = new Gambo_LuaFuncs();
            // LuaScript.Globals["PieceType"] 
            // LuaScript.Globals["PieceType"] = new Dictionary<string, PieceType>
            // {
            //     { "PAWN", PieceType.PAWN },
            //     { "ROOK", PieceType.ROOK },
            //     { "KNIGHT", PieceType.KNIGHT },
            //     { "BISHOP", PieceType.BISHOP },
            //     { "QUEEN", PieceType.QUEEN },
            //     { "KING", PieceType.KING },
            // };
            

            {
                debug_object = new GameObject("Lua_Debug_Object");
                UnityEngine.Object.DontDestroyOnLoad(debug_object);
                debug_object.transform.position = Vector3.zero;
            }


            {


                string testscript = File.ReadAllText(Path.Combine(pluginPath, "PreLaunch.lua"));

                
                LuaScript.DoString(testscript);
            }
            
        }

        private static void RegisterTypesLua()
        {
            UserData.RegisterType<Gambo_LuaFuncs>();
            UserData.RegisterType<Vector2Int>();
            UserData.RegisterType<TileBehaviour>();
            // UserData.RegisterType<PieceType>();
            
        }
    }
    
    

    public class Gambo_LuaFuncs
    {
        public static BaseGambit currentGambit;
        
        public int fibonacci(int n)
        {
            int a = 0;
            int b = 1;
            int c = 1;
            for (int i = 0; i < n; i++)
            {
                c = a + b;
                a = b;
                b = c;
            }

            return c;
        }

        public Vector2Int GetBoardSize()
        {
            Vector2Int v = Vector2Int.zero;
            v.x = BoardManager.Instance.Board.GetLength(0);
            v.y = BoardManager.Instance.Board.GetLength(1);
            
            return v;
        }

        public TileBehaviour GetTile(int x, int y)
        {
            return BoardManager.Instance.Board[x, y];
        }
        
        
        
        public bool AddStockPiece(int type, bool golden, bool ghost, bool onBoard)
        {
            SingletonMonoBehaviour<StockManager>.Instance.AddPiece((PieceType)type, GamboLuaManager.debug_object.transform.position, golden, false, ghost, onBoard);
            return SingletonMonoBehaviour<StockManager>.Instance.RoomAvailable();
        }
        
        public int GiveGold(int goldAmount)
        {
            
            
            try
            {
                if (currentGambit != null)
                {
                    SingletonMonoBehaviour<MoneyAnimationManager>.Instance.SpawnMoney(currentGambit.transform,
                        goldAmount);
                }
                else
                {
                    
                    GamboLuaManager.debug_object.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    SingletonMonoBehaviour<MoneyAnimationManager>.Instance.SpawnMoney(GamboLuaManager.debug_object.transform, goldAmount);
                }

                SingletonMonoBehaviour<ChessDataManager>.Instance.IncreaseCoin(goldAmount);
                return 0;
            }
            catch (Exception e)
            {
                PluginMain._logger.LogError(e);
            }
            return 0;
        }
    }
    
    public class GamboLuaGambit : BaseGambit
    {
        
        private void Start()
        {
            // PieceManager instance = SingletonMonoBehaviour<PieceManager>.Instance;
            // instance.OnPhantomPieceDisappear = (Action<BasePieceBehaviour>)Delegate.Combine(instance.OnPhantomPieceDisappear, new Action<BasePieceBehaviour>(EarnMoney));
            // GameManager instance2 = SingletonMonoBehaviour<GameManager>.Instance;
            // instance2.onStateChanged = (Action<State>)Delegate.Combine(instance2.onStateChanged, new Action<State>(Reset));
        }
        
        public override void Trigger()
        {
            return;
        }
        
        
    }
}
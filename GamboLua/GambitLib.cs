using System.Collections.Generic;
using System.Reflection;
using Blukulele.CHE;
using Blukulele.Core;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace Gambo
{
    public class GambitLib
    {

        public static void testnewgambit()
        {
            var library = SingletonMonoBehaviour<GambitLibrary>.Instance;

            for (int i = 0; i < 10; i++)
            {
                
                var new_so = ScriptableObject.Instantiate(library.GambitsInfo[11]);
                new_so.ID = $"{888 + i}";
                new_so.name = $"----{888 + i}";
                new_so.GambitName = $"----{888 + i}";
                
                RegisterGambit(new_so);
                
            }
            CLearLib();
            
        }
        
        public static int RegisterGambit(SO_Gambit gambit)
        {
            var library = SingletonMonoBehaviour<GambitLibrary>.Instance;
            
            LocalizationManager locamgr = SingletonMonoBehaviour<LocalizationManager>.Instance;
            
            GambitBehaviour behaviour_template = library.Gambits[0];
            
            string gambit_name = gambit.GambitName;
            string gambit_desc = gambit.GambitDescription;
            
            
            
            string name_key= $"lua_gambit_{gambit.ID}_name";
            string desc_key= $"lua_gambit_{gambit.ID}_description";

            var traduction = locamgr.GetTraduction();
            
            JSONNode gambitNode = traduction["gambit"];
            
            gambitNode[name_key] = gambit_name;
            gambitNode[desc_key] = gambit_desc;

            gambit.GambitName = name_key;
            gambit.GambitDescription = desc_key;
            
            
            
            
            GambitBehaviour new_behaviour = Object.Instantiate(behaviour_template);
            
            new_behaviour.Info = gambit;
            
            
            gambit.Gambit_Library_Index = library.GambitsInfo.Count;
            library.GambitsInfo.Add(gambit);
            
            library.Gambits.Add(new_behaviour);
                
            var um = SingletonMonoBehaviour<GambitUnlockManager>.Instance;
            um.UnlockGambit(gambit.ID);
                
            
            return 0;
        }

        public static void CLearLib()
        {
            var gamblib = SingletonMonoBehaviour<GambitLibrary>.Instance;
            var initMethod = typeof(GambitLibrary).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Instance);
            
            gamblib.Gambits_Common.Clear();
            gamblib.Gambits_Rare.Clear();
            gamblib.Gambits_Epic.Clear();
            gamblib.Gambits_Legendary.Clear();
            
            gamblib.Gambit_PAWN.Clear();
            gamblib.Gambit_KNIGHT.Clear();
            gamblib.Gambit_BISHOP.Clear();
            gamblib.Gambit_ROOK.Clear();
            gamblib.Gambit_QUEEN.Clear();
            gamblib.Gambit_KING.Clear();
            
            gamblib.Gambit_MONEY.Clear();
            gamblib.Gambit_OTHER.Clear();
            gamblib.Gambit_PROMOTION.Clear();
            gamblib.Gambit_WAIT.Clear();
            gamblib.Gambit_PHANTOM.Clear();
            gamblib.Gambit_BLESS.Clear();
            gamblib.Gambit_PROTECTIVE.Clear();
            gamblib.Gambit_TRAP.Clear();
            gamblib.Gambit_GOLDEN.Clear();
            gamblib.Gambit_LAND.Clear();
            gamblib.Gambit_SACRIFICE.Clear();
            gamblib.Gambit_PIECE_SELLER.Clear();
            gamblib.Gambit_CRUMBLE.Clear();
            gamblib.Gambit_GAMBIT_SELLER.Clear();

            if (initMethod != null)
            {
                initMethod.Invoke(gamblib, null);
                PluginMain._logger.LogInfo("Reinitialized Gambit Library");
            }
        }
    }
    
    
    public static class Patches
    {
        
        static readonly FieldInfo f_Index =
            AccessTools.Field(typeof(GambitCollectionSlide), "m_Index");

        static readonly FieldInfo f_Orderer =
            AccessTools.Field(typeof(GambitCollectionSlide), "m_GambitOrderer");

        static readonly FieldInfo f_Hints =
            AccessTools.Field(typeof(GambitCollectionSlide), "m_Hints");

        static readonly MethodInfo m_UpdateUI =
            AccessTools.Method(typeof(GambitCollectionSlide), "UpdateUI");

        static readonly FieldInfo f_GambitCountText =
            AccessTools.Field(typeof(RunInfoCanvas), "m_TXT_GambitCount");

        
        [HarmonyPatch(typeof(GambitCollectionSlide), "IncreaseIndex")]
        [HarmonyPostfix]
        static void Increase_Postfix(GambitCollectionSlide __instance)
            => FixPagination(__instance);

        [HarmonyPatch(typeof(GambitCollectionSlide), "DecreaseIndex")]
        [HarmonyPostfix]
        static void Decrease_Postfix(GambitCollectionSlide __instance)
            => FixPagination(__instance);

        
        static void FixPagination(GambitCollectionSlide slide)
        {
            var orderer = f_Orderer.GetValue(slide) as List<SO_Gambit>;
            if (orderer == null || orderer.Count == 0) return;

            int count = orderer.Count;
            int vanillaPages = count / 10;
            int realPages = Mathf.CeilToInt(count / 10f);
            
            EnsureHints(slide, realPages);

            if (realPages == vanillaPages) return;

            int current = (int)f_Index.GetValue(slide);
            
            if (current == 0 || current == vanillaPages - 1)
            {
                f_Index.SetValue(slide, realPages - 1);
                m_UpdateUI.Invoke(slide, null);
            }
        }

        static void EnsureHints(GambitCollectionSlide slide, int required)
        {
            var hints = f_Hints.GetValue(slide) as List<HintCircleBehaviour>;
            if (hints == null || hints.Count >= required) return;

            var template = hints[hints.Count - 1];
            var parent = template.transform.parent;

            while (hints.Count < required)
            {
                var clone = Object.Instantiate(template, parent);
                hints.Add(clone);
            }
        }

        
        [HarmonyPatch(typeof(RunInfoCanvas), "ComputeGambitCount")]
        [HarmonyPrefix]
        static bool GambitCountPrefix(RunInfoCanvas __instance)
        {
            var txt = f_GambitCountText.GetValue(__instance) as TMP_Text;
            var lib = SingletonMonoBehaviour<GambitLibrary>.Instance;

            if (txt != null && lib != null && txt.text != null)
                txt.text = $"{lib.GambitsInfo.Count}/200";

            return false; 
        }
    }
}
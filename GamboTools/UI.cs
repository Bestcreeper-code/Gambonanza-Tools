using System;
using System.Collections.Generic;
using Blukulele.CHE;
using Blukulele.Core;
using UnityEngine;

namespace Gambo
{
    public static class UI
    {
        private static List<SO_Gambit> gambits = new List<SO_Gambit>();
        private static string[] gambit_names;

        private static string search = "";
        private static int selectedIndex = -1;

        private static Rect searchRect = new Rect(20, 20, 250, 180);
        private static Rect piecesRect = new Rect(20, 200, 150, 180);
        private static Rect moneyRect = new Rect(20, 380, 100, 80);
        private static Rect tilesRect = new Rect(20, 560, 200, 380);
        private static Rect pieceposRect = new Rect(300, 20, 200, 380);
        
        
        public static bool show_window = false;

        private static GUIStyle labelStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle toggleStyle;
        private static GUIStyle textFieldStyle;

        private static bool stylesInitialized = false;

        private static void InitStyles()
        {
            if (stylesInitialized) return;

            Font taken = null;

            foreach (var text in UnityEngine.Object.FindObjectsOfType<UnityEngine.UI.Text>())
            {
                if (text.font != null)
                {
                    taken = text.font;
                    break;
                }
            }

            if (taken == null)
            {
                foreach (var tmp in UnityEngine.Object.FindObjectsOfType<TMPro.TMP_Text>())
                {
                    if (tmp.font?.sourceFontFile != null)
                    {
                        taken = tmp.font.sourceFontFile;
                        break;
                    }
                }
            }

            labelStyle = new GUIStyle();
            labelStyle.font = taken;
            labelStyle.fontSize = 13;
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.padding = new RectOffset(4, 4, 2, 2);

            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.font = taken;
            buttonStyle.fontSize = 13;
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.white;
            
            toggleStyle = new GUIStyle(GUI.skin.toggle);
            toggleStyle.font = taken;
            toggleStyle.fontSize = 13;
            toggleStyle.normal.textColor = Color.red;
            toggleStyle.hover.textColor = Color. red;
            toggleStyle.active.textColor = Color.green;
            
            
            textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.font = taken;
            textFieldStyle.fontSize = 13;
            textFieldStyle.normal.textColor = Color.white;

            stylesInitialized = true;
        }

        public static void Init()
        {
            JSONNode translation = SingletonMonoBehaviour<LocalizationManager>.Instance.GetTraduction();
            var list = SingletonMonoBehaviour<GambitLibrary>.Instance.GambitsInfo;

            foreach (var g in list)
                gambits.Add(g);

            gambit_names = new string[gambits.Count];

            for (int i = 0; i < gambits.Count; i++)
                gambit_names[i] = translation["gambit"][gambits[i].GambitName];
        }

        public static void Render()
        {
            if (!show_window) return;

            GUISkin savedSkin = GUI.skin;
            GUI.skin = null;

            InitStyles();

            searchRect = GUI.Window(0, searchRect, DrawSearch, "Search Gambits");

            piecesRect = GUI.Window(1, piecesRect, DrawPiecesSelect, "Select Pieces");
            
            moneyRect = GUI.Window(2, moneyRect, DrawMoneySelect, "$");
            
            tilesRect = GUI.Window(3, tilesRect, DrawTilesSelect, "Select tiles");

            pieceposRect = GUI.Window(4, pieceposRect, DrawPieceSelect, "Select Piece Pos and Mods");
            
            GUI.skin = savedSkin;
        }

        private static void DrawSearch(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Search:", labelStyle);
            search = GUILayout.TextField(search, textFieldStyle);

            int shown = 0;

            for (int i = 0; i < gambit_names.Length; i++)
            {
                if (shown >= 4) break;

                if (!string.IsNullOrEmpty(search) &&
                    !gambit_names[i].ToLower().Contains(search.ToLower()))
                    continue;

                if (GUILayout.Button(gambit_names[i], buttonStyle))
                {
                    search = gambit_names[i];
                    Helpers.GiveGambit(gambits[i]);
                }

                shown++;
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private static PieceType selected_piece = PieceType.PAWN;
        private static void DrawPiecesSelect(int id)
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("To Stock", buttonStyle))
            {
                Helpers.GivePiece(selected_piece);
            }

            foreach (var pieceT in Enum.GetValues(typeof(PieceType)))
            {
                if ((PieceType)pieceT == PieceType.NONE) break;


                bool selected = (PieceType)pieceT == selected_piece;
                if (GUILayout.Toggle(selected, pieceT.ToString(), toggleStyle))
                {
                    selected_piece = (PieceType)pieceT;
                }
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private static string[] tile_commands = new[]
        {
            "Trap",
            "Phantom",
            "Golden",
            "Protected",
            "Blessed",
            "Cursed",
            "Shaking",
            "Fell",
            "NotFell",
            "NotShaking"
        };
        private static string[] w_piece_commands = new[]
        {
            "Phantom",
            "Golden",
            "Protected",
            "Blessed"
        };
        private static string[] b_piece_commands = new[]
        {
            "Elite",
            "Polymorph",
            "Crumbler",
            "Invoker",
            "Stasis"
        };

        private static int tile_select_x;
        private static int tile_select_y;
        
        private static bool is_all_tiles = false;
        private static void DrawTilesSelect(int id)
        {
            GUILayout.BeginVertical();
            
            GUILayout.Label($"x pos {tile_select_x+1}", labelStyle);
            tile_select_x = Mathf.RoundToInt(GUILayout.HorizontalSlider(tile_select_x, 0, 7));
            GUILayout.Label($"y pos {tile_select_y}", labelStyle);
            tile_select_y = Mathf.RoundToInt(GUILayout.HorizontalSlider(tile_select_y, 1, 5));

            is_all_tiles = GUILayout.Toggle(is_all_tiles, "All Tiles?", toggleStyle);
            
            
            foreach (string command in tile_commands)
            {
                if (GUILayout.Button(command, buttonStyle))
                {
                    if (is_all_tiles)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            for (int y = 0; y < 5; y++)
                            {
                                Helpers.SetTile(x, y, command);
                            }
                        }
                    }
                    else
                    {
                        Helpers.SetTile(tile_select_x, 5-tile_select_y, command);
                    }
                }
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        
        private static int piece_select_x;
        private static int piece_select_y;
        
        private static bool is_white = false;

        private static void DrawPieceSelect(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("Select piece in the Piece window",labelStyle);
            
            GUILayout.Label($"x pos {piece_select_x + 1}", labelStyle);
            piece_select_x = Mathf.RoundToInt(GUILayout.HorizontalSlider(piece_select_x, 0, 7));
            GUILayout.Label($"y pos {piece_select_y}", labelStyle);
            piece_select_y = Mathf.RoundToInt(GUILayout.HorizontalSlider(piece_select_y, 1, 5));

            is_white = GUILayout.Toggle(is_white, "White?", toggleStyle);

            
            string[] commands_list = is_white? w_piece_commands : b_piece_commands;

            foreach (string command in commands_list)
            {
                if (GUILayout.Button(command, buttonStyle))
                {
                    Helpers.PlacePiece(piece_select_x, 5-piece_select_y, is_white,selected_piece, command);
                }
            }
            
            
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private static string moneyTxt = "";
        private static void DrawMoneySelect(int id)
        {
            GUILayout.BeginVertical();
            moneyTxt = GUILayout.TextField(moneyTxt, textFieldStyle);

            if (GUILayout.Button("Give $", buttonStyle))
            {
                int goldAmount = moneyTxt.ToInt();
                SingletonMonoBehaviour<MoneyAnimationManager>.Instance
                    .SpawnMoney(PluginUpdater.Instance.gameObject.transform, goldAmount);
                SingletonMonoBehaviour<ChessDataManager>.Instance.IncreaseCoin(goldAmount);
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        
    }
}
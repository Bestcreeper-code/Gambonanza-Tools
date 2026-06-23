using Blukulele.Audio;
using Blukulele.CHE;
using Blukulele.Core;
using Blukulele.Module.Audio;
using DG.Tweening;
using UnityEngine;

namespace Gambo
{
    public static class Helpers
    {

        public static int GiveGambit(SO_Gambit gambit)
        {
            if (SingletonMonoBehaviour<GambitManager>.Instance.IsFull()) return -1;
            
            GambitPlaceBehaviour gambitPlace = SingletonMonoBehaviour<GambitManager>.Instance.GetGambitPlace();
                
            GambitBehaviour gambitBehaviour2 = (
                gambitPlace.CurrentGambit = UnityEngine.Object.Instantiate
                (
                    SingletonMonoBehaviour<GambitLibrary>.Instance.Gambits[
                        SingletonMonoBehaviour<GambitLibrary>.Instance.GambitsInfo.IndexOf(gambit)
                    ],
                    Vector3.zero, Quaternion.identity, 
                    gambitPlace.GambitParent
                )
            );
            gambitBehaviour2.transform.localScale = Vector3.zero;
            gambitBehaviour2.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            gambitBehaviour2.transform.DOFollow(gambitPlace.GambitParent, 0.2f).SetDelay(0.3f);
            SingletonMonoBehaviour<GambitManager>.Instance.OnGetGambit?.Invoke();
            return 0;
        }

        public static int GivePiece(PieceType piece)
        {
            if (!SingletonMonoBehaviour<StockManager>.Instance.RoomAvailable()) return -1;
            
            SingletonMonoBehaviour<StockManager>.Instance.AddPiece(piece, Vector3.zero);
            return 0;

        }

        public static int SetTile(int x, int y, string type)
        {
            
            switch (type)
            {
                case "Unmodified":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].TurnToDefault();
                    break;
                }
                case "Trap":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].TurnToHunter(true);
                    break;
                }
                case "Phantom":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].TurnToPhantom(true);
                    break;
                }
                case "Golden":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].TurnToGold(true);
                    break;
                }
                case "Protected":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].TurnToShield(true); 
                    break;
                }
                case "Blessed":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].TurnToBenediction(true);
                    break;
                }
                case "Cursed":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].Curse();
                    break;
                }
                case "Shaking":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].IsShaking = true;
                    break;
                }
                case "Fell":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].Fall();
                    break;
                }
                case "NotFell":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].HasFell = false;
                    break;
                }
                case "NotShaking":
                {
                    SingletonMonoBehaviour<BoardManager>.Instance.Board[y, x].IsShaking = false;
                    break;
                }
            }

            return 0;
        }

        public static int PlacePiece(int x, int y, bool iswhite, PieceType piece, string mod)
        {
            TileBehaviour[,] Board = SingletonMonoBehaviour<BoardManager>.Instance.Board;
            
            if (Board[y, x] == null || Board[y,x].PlaceToPutPieces == null)
            {
                return -1;
            }
            AudioManager.Play(AudioEvents.Apparition);
            BasePieceBehaviour basePieceBehaviour = Object.Instantiate(SingletonMonoBehaviour<Library>.Instance.GetPiece(piece, iswhite? PieceColor.WHITE:PieceColor.BLACK), Board[y,x].PlaceToPutPieces.position, Quaternion.identity, Board[y, x].PlaceToPutPieces);
            Board[y, x].Piece = basePieceBehaviour;
            basePieceBehaviour.CurrentTile = Board[y, x];

            if (!iswhite)
            {
                switch (mod)
                {
                    case "Elite":
                    {
                        basePieceBehaviour.EnemyAbilityModifier.Boss();
                        break;
                    }
                    case "Polymorph":
                    {
                        basePieceBehaviour.EnemyAbilityModifier.Magician();
                        break;
                    }
                    case "Crumbler":
                    {
                        basePieceBehaviour.EnemyAbilityModifier.Crumbler();
                        break;
                    }
                    case "Invoker":
                    {
                        basePieceBehaviour.EnemyAbilityModifier.Invoker();
                        break;
                    }
                    case "Stasis":
                    {
                        basePieceBehaviour.EnemyAbilityModifier.ClockBossPower();
                        break;
                    }
                }
            }
            else
            {
                switch (mod)
                {
                    case "Phantom":
                    {
                        
                        basePieceBehaviour.Modifier.ResetPhantom();
                        basePieceBehaviour.Modifier.ResetStatus();
                        basePieceBehaviour.Modifier.TurnToPhantom();
                        basePieceBehaviour.VisualEffect.TurnToPhantom();
                        basePieceBehaviour.CurrentTile.TileVisual.PhantomEffect();
                        break;
                    }
                    case "Golden":
                    {
                        basePieceBehaviour.Modifier.ResetGold();
                        basePieceBehaviour.Modifier.ResetStatus();
                        basePieceBehaviour.Modifier.TurnToGold();
                        basePieceBehaviour.VisualEffect.TurnToGold();
                        basePieceBehaviour.CurrentTile.TileVisual.GoldEffect();
                        break;
                    }
                    case "Protected":
                    {
                        basePieceBehaviour.Modifier.Protect();
                        break;
                    }
                    case "Blessed":
                    {
                        basePieceBehaviour.Modifier.Benediction();
                        break;
                    }
                }
            }
            
            
            return 0;
        }
    }
}

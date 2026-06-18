using Blukulele.CHE;
using Blukulele.Core;
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
        
    }
}
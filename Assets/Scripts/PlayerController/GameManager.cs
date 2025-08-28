using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class GameManager : MonoBehaviour
    {
        private void Awake()
        {
            Messenger.Default.Subscribe<CaughtPlayerPayload>(OnCaughtPlayer);
            Messenger.Default.Subscribe<CatchKeyPayload>(OnCatchKey);
        }
        
        private void OnCaughtPlayer(CaughtPlayerPayload payload)
        {
            Debug.Log("Lose");
            NotiEndGame();
        }
        
        private void OnCatchKey(CatchKeyPayload payload)
        {
            Debug.Log("Win");
            NotiEndGame();
        }

        private void NotiEndGame()
        {
            Messenger.Default.Publish(new GameStateUpdatePayload(GameState.EndGame));
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<CaughtPlayerPayload>(OnCaughtPlayer);
            Messenger.Default.Unsubscribe<CatchKeyPayload>(OnCatchKey);
        }
    }
}
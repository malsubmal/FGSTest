using System;
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

        private void Start()
        {
            Messenger.Default.Publish(new GameStateUpdatePayload(GameState.Running));
        }

        private void OnCaughtPlayer(CaughtPlayerPayload payload)
        {
            NotiEndGame(false);
        }
        
        private void OnCatchKey(CatchKeyPayload payload)
        {
            NotiEndGame(true);
        }

        private void NotiEndGame(bool isWin)
        {
            Messenger.Default.Publish(new GameStateUpdatePayload(GameState.EndGame));
            Messenger.Default.Publish(new GameResultPayload(isWin));
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<CaughtPlayerPayload>(OnCaughtPlayer);
            Messenger.Default.Unsubscribe<CatchKeyPayload>(OnCatchKey);
        }
    }
}
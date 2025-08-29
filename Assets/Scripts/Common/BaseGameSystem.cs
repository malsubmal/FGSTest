using System;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.Common
{
    public class BaseGameSystem : MonoBehaviour
    {
        [SerializeField] private bool _isSystemActive;
        public bool IsSystemActive => _isSystemActive;

        private void Awake()
        {
            Messenger.Default.Subscribe<GameStateUpdatePayload>(OnGameStateUpdate);
            SystemAwake();
        }
        
        protected virtual void SystemAwake()
        {
        }

        private void Update()
        {
            if (!_isSystemActive) return;
            
            SystemUpdate();
        }
        
        protected virtual void SystemUpdate()
        {
        }
        
        private void FixedUpdate()
        {
            if (!_isSystemActive) return;
            
            SystemFixedUpdate();
        }
        
        protected virtual void SystemFixedUpdate()
        {
        }

        private void OnGameStateUpdate(GameStateUpdatePayload payload)
        {
            _isSystemActive = payload.GameState == GameState.Running;
        }
        
        
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<GameStateUpdatePayload>(OnGameStateUpdate);
            SystemDestroy();
        }
        
        protected virtual void SystemDestroy()
        {
            
        }
    }
}
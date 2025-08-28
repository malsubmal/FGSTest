using System;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.Environment
{
    public class KeyObject : MonoBehaviour
    {
        [SerializeField] private bool _isActive;
        private Func<Collider, bool> _isColliderPlayer;

        public void ActivateCollider()
        {
            _isActive = true;
        }
        
        public void InitPlayerColliderChecker(Func<Collider, bool> checker)
        {
            _isColliderPlayer = checker;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isActive) return;
            
            if (_isColliderPlayer == null) return;
            
            Debug.Log($"Collision Enter {other.name}");
            
            if (_isColliderPlayer(other))
                ProcessCatchKey();
        }
        
        private void ProcessCatchKey()
        {
            Messenger.Default.Publish(new CatchKeyPayload());
            this.gameObject.SetActive(false);
        }
    }
}
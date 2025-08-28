using FGSTest.Payload;
using SuperMaxim.Messaging;
using Unity.Mathematics;
using UnityEngine;

namespace FGSTest.Enemy
{
    public class PlayerTracker : MonoBehaviour
    {
        [SerializeField] private float _toleranceRange;
        
        public Vector3 PlayerPosition => _playerTransform.position;
        public float PlayerRadius => _playerRadius;
        
        private float _playerRadius;
        
        private Transform _playerTransform;
        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerSpawnPayload>(OnPlayerSpawn);
        }
        
        private void OnPlayerSpawn(PlayerSpawnPayload payload)
        {
            _playerTransform = payload.PlayerTransform;
            _playerRadius = payload.PlayerRadius;
        }

        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerSpawnPayload>(OnPlayerSpawn);
        }
    }
}
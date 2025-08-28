using System;
using FGSTest.Common;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FGSTest.Environment
{
    public class KeySpawner : BaseGameSystem
    {
        [Header("Position Config")] 
        [SerializeField] private float _initialRadius;
        [SerializeField] private float _subsequentRadius;
        [SerializeField] private float _moveInterval;
        [SerializeField] private MapConstraint _mapConstraint;
        [SerializeField] private LayerMask raycastMask;
        
        [Header("Key Prefab")]
        [SerializeField] private GameObject _keyPrefab;
        
        [Header("Movement State")]
        [SerializeField] private MovementState _movementState;
        private GameObject _keyInstance;

        [Serializable]
        public struct MovementState
        {
            public float CountdownMovePos;
            
            public MovementState SetDefault()
            {
                CountdownMovePos = 0;
                return this;
            }
            
            public MovementState SetCountdownMovePos(float pos)
            {
                CountdownMovePos = pos;
                return this;
            }
        }

        protected override void SystemAwake()
        {
            base.SystemAwake();
            Messenger.Default.Subscribe<PlayerSpawnPayload>(OnPlayerSpawn);
        }

        private void OnPlayerSpawn(PlayerSpawnPayload payload)
        {
            InstantiateKey( (col) => col.GetInstanceID() == payload.ColliderInstanceId);
            RandKeyPosition(_initialRadius);
            _movementState = _movementState.SetCountdownMovePos(_moveInterval);
        }

        private void Update()
        {
            if (!IsSystemActive || _keyInstance == null) return;
            
            _movementState = _movementState.SetCountdownMovePos(_movementState.CountdownMovePos - Time.deltaTime);

            if (_movementState.CountdownMovePos <= 0.0f)
            {
                RandKeyPosition(_subsequentRadius);
                _movementState = _movementState.SetCountdownMovePos(_moveInterval);
            }
        }
        
        private void RandKeyPosition(float minRadius = 0, Vector3 offset = default)
        {
            var pos = ComputeKeyPosition(minRadius, offset);

            if (!_keyInstance)
                return;
            
            _keyInstance.transform.position = pos;
            
        }
        
        private void InstantiateKey(Func<Collider, bool> checker)
        {
            _keyInstance = Instantiate(_keyPrefab, Vector3.zero, Quaternion.identity);
            var hasComp = _keyInstance.TryGetComponent(out KeyObject keyObject);

            if (!hasComp)
            {
                Debug.Log("No key object Component");
                return;
            }
            
            keyObject.InitPlayerColliderChecker(checker);
            keyObject.ActivateCollider();
        }

        private Vector3 ComputeKeyPosition(float minRadius = 0, Vector3 offset = default)
        {
            var pos = _mapConstraint.SampleRandomPosInsideMesh();
            
            if (minRadius > 0)
                pos = ClampMinimumDistance(pos, offset, minRadius);
            
            pos = _mapConstraint.ClampXZPosToMap(pos);
     
            pos.y = 50;
            
            Debug.Log($"Key position {pos}");
            
            var ray = new Ray(pos, Vector3.down);

            if (Physics.Raycast(ray, out var hit, 100, 
                    layerMask: raycastMask))
            {
                pos = hit.point;
                return pos;
            }

            Debug.Log("No valid position found");
            return Vector3.zero;
        }
        
        private Vector3 ClampMinimumDistance(Vector3 pos, Vector3 target, float minDistance)
        {
            float minDistanceSqr = minDistance * minDistance;

            Vector3 delta = new Vector3(pos.x - target.x, 0f, pos.z - target.z);
            float distSqr = delta.sqrMagnitude;

            if (distSqr < minDistanceSqr)
            {
                var direction = delta.normalized;
                if (direction == Vector3.zero) direction = Vector3.forward; 
                Vector3 offset = direction * minDistance;
                pos.x = target.x + offset.x;
                pos.z = target.z + offset.z;
            }

            return pos;
        }

        protected override void SystemDestroy()
        {
            base.SystemDestroy();
            Messenger.Default.Unsubscribe<PlayerSpawnPayload>(OnPlayerSpawn);
        }
    }
}

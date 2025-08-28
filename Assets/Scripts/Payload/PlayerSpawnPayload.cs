using System;
using UnityEngine;

namespace FGSTest.Payload
{
    [Serializable]
    public struct PlayerSpawnPayload
    {
        public int ColliderInstanceId;
        public Transform PlayerTransform;
        public float PlayerRadius;
        
        public PlayerSpawnPayload(Transform playerTransform, float playerRadius, int colliderInstanceId)
        {
            PlayerTransform = playerTransform;
            PlayerRadius = playerRadius;
            ColliderInstanceId = colliderInstanceId;
        }
    }
}
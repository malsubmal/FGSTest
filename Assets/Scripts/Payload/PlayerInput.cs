using System;
using UnityEngine;

namespace FGSTest.Payload
{
    [Serializable]
    public struct PlayerInput
    {
        public Vector2 Movement;
        public bool IsJumpPressed;
        public bool IsAccelerationPressed;
    }
    
    public interface IGetPlayerInput
    {
        public PlayerInput PlayerInput { get; }
    }

    [Serializable]
    public struct PlayerInputPayload
    {
        public PlayerInput PlayerInput;
        
        public PlayerInputPayload(PlayerInput playerInput)
        {
            PlayerInput = playerInput;
        }
    }
}
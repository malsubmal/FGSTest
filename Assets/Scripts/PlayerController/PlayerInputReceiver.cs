using System;
using FGSTest.Payload;
using Unity.Collections;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class PlayerInputReceiver : MonoBehaviour, IGetPlayerInput
    {
        [SerializeField] private CharacterIdentityHolder _idHolder;
        [SerializeField, ReadOnly] private PlayerInput _playerInput;
        public PlayerInput PlayerInput => _playerInput;
        private void Update()
        {
            var playerInput =  new PlayerInput
            {
                IsJumpPressed = Input.GetKeyDown(KeyCode.O),
                IsAccelerationPressed = Input.GetKeyDown(KeyCode.P),
                Movement = new Vector2(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical")).normalized
            };
            
            _playerInput = playerInput;
            
        }
    }
}
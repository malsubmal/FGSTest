using FGSTest.Common;
using FGSTest.Payload;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class CheckGroundState : IStateUpdater
    {
        [SerializeField] private CharacterIdentityHolder _idHolder;
        [SerializeField] private CharacterController _characterController;
        private CharacterState _groundedState;
        private void Update()
        {
            _groundedState = _characterController.isGrounded ? CharacterState.Grounded : CharacterState.Idle;
        }

        public override CharacterState Mask => CharacterState.Grounded;
        public override CharacterState CharacterState => _groundedState;
    }
}
using System;
using FGSTest.Common;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace FGSTest.PlayerController
{
    public class MovementAction : CharacterAction
    {
        [Header("Walking Movement Config")]
        [SerializeField] private float _walkingAcceleration;
        
        [SerializeField] private CharacterController _characterController;
        [SerializeField, ReadOnly] private MovementState _curState;
        [SerializeField] private PlayerInputReceiver _playerInput;

        [Header("Cache Value")]
        [SerializeField] private float _maxWalkSpeed;
        
        [Serializable]
        public struct MovementState
        {
            public float CurrentSpeed;
            public Vector2 CurrentMovementDirection;
            
            public MovementState SetDefault()
            {
                CurrentSpeed = 0;
                CurrentMovementDirection = Vector2.zero;
                return this;
            }
            
            public MovementState SetCurrentSpeed(float speed)
            {
                CurrentSpeed = speed;
                return this;
            }
            
            public MovementState SetCurrentMovementDirection(Vector2 dir)
            {
                CurrentMovementDirection = dir;
                return this;
            }
        }
        
        private void Start()
        {
            Messenger.Default.Subscribe<CharacterStatUpdatePayload>(OnCharacterStatUpdate);
        }
        
        private void OnCharacterStatUpdate(CharacterStatUpdatePayload payload)
        {
            if (payload.StatType == CharacterStatType.Speed)
            {
                _curState = _curState.SetCurrentSpeed(payload.Value);
            }
        }
        
        protected override void DestroyAction()
        {
            Messenger.Default.Unsubscribe<CharacterStatUpdatePayload>(OnCharacterStatUpdate);
        }
        
        protected override void UpdateAction()
        {
            var input = _playerInput.PlayerInput;
            
            if (_curState.CurrentSpeed > _maxWalkSpeed)
                _curState = _curState.SetCurrentSpeed(_maxWalkSpeed);
            
            if (input.Movement == Vector2.zero)
            {
                ClearMovement();
                return;
            }
            
            var dotValue = input.Movement.magnitude;
            
            if (_curState.CurrentMovementDirection != Vector2.zero)
            {
                dotValue = Vector2.Dot(input.Movement, _curState.CurrentMovementDirection);
                if (dotValue <= 0.0f || input.Movement == Vector2.zero)
                {
                    //Decelerate(); //just straight up stop for now
                    ClearMovement();
                    return;
                }
            }

            _curState = _curState.SetCurrentMovementDirection(input.Movement);
            Accelerate(dotValue);
        }

        private void ClearMovement()
        {
            _curState = _curState.SetDefault();
            Messenger.Default.Publish(
                new CharacterStateUpdateRequestPayload(StateModificationType.Remove, 
                    CharacterState.Walk,
                    _idHolder.CharacterId));
        }

        private void Accelerate(float dotDir)
        {
            if (dotDir < 0.85f)
                _curState = _curState.SetCurrentSpeed(_curState.CurrentSpeed * dotDir);
            else
                _curState = _curState.SetCurrentSpeed(Mathf.Min(_curState.CurrentSpeed + _walkingAcceleration * Time.deltaTime, _maxWalkSpeed));
            
            var moveDirXZ = new Vector3(_curState.CurrentMovementDirection.x, 0, _curState.CurrentMovementDirection.y);
            
            _characterController.Move(_curState.CurrentSpeed * Time.deltaTime * moveDirXZ.normalized);
            
            Messenger.Default.Publish(
                new CharacterStateUpdateRequestPayload(StateModificationType.Add, 
                    CharacterState.Walk,
                    _idHolder.CharacterId));
        }

        protected override void OnCancelAction()
        {
            base.OnCancelAction();
            _curState = _curState.SetDefault();
        }
    }


    [Serializable]
    public struct CharacterStatValue
    {
        public CharacterStatType StatType;
        public float Value;
    }
}
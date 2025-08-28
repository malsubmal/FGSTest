using System;
using System.Collections.Generic;
using FGSTest.Common;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using Unity.Collections;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class JumpAction : CharacterAction
    {
        [Header("Start Jump")] 
        [SerializeField] private AnimationCurve _jumpCurve;
        [SerializeField] private float _jumpHeight;
        [SerializeField] private float _jumpTime;
        [SerializeField] AnimationKey _jumpKey = AnimationKey.Jump;
        
        [Header("In air")] 
        [SerializeField] private float _timeInAir;

        [Header("Landing")] 
        [SerializeField] private AnimationCurve _fallCurve;
        [SerializeField] private float _fallTime;
        [SerializeField] AnimationKey _fallKey = AnimationKey.Landing;

        [Serializable]
        public enum JumpState : byte
        {
            None = 0,
            StartJump = 1,
            InAir = 2,
            Landing = 3,
        }

        [Header("Current State")] 
        [SerializeField, ReadOnly] private JumpCurrentState _curState;

        [Header("Reference")] 
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerInputReceiver _playerInput;
        
        [SerializeField] CharacterAnimationHandler _animationHandler;

        [Serializable]
        public struct JumpCurrentState 
        {
            public JumpState CurrentState;
            public float CurrentStateTime;
            public float StartHeight;
            
            public JumpCurrentState SetDefault()
            {
                CurrentState = JumpState.None;
                CurrentStateTime = 0;
                StartHeight = 0;
                return this;
            }
            
            public JumpCurrentState SetStateTime(float time)
            {
                CurrentStateTime = time;
                return this;
            }

            public JumpCurrentState SetStartHeight(float height)
            {
                StartHeight = height;
                return this;
            }
            
            public JumpCurrentState SetCurrentState(JumpState state)
            {
                CurrentState = state;
                return this;
            }
        }
        
        // #region State Machine Config
        //
        // private Dictionary<JumpState, Action<JumpCurrentState>> _stateUpdateTable;
        // private Dictionary<JumpState, Func<JumpCurrentState, bool>> _checkStateChangeTable;
        // private Dictionary<JumpState, Func<JumpCurrentState, JumpCurrentState>> _stateChangeTable;
        // #endregion
        //
        // private void InitStateMachine()
        // {
        //     _stateUpdateTable = new()
        //     {
        //         { JumpState.None, JumpNone },
        //         { JumpState.StartJump, UpdateStartJump },
        //         { JumpState.InAir, JumpNone },
        //         { JumpState.Landing, UpdateLanding },
        //     };
        //
        //     _checkStateChangeTable = new()
        //     {
        //         { JumpState.None, state => false },
        //         { JumpState.StartJump, IsStateTimeout },
        //         { JumpState.InAir, IsStateTimeout },
        //         { JumpState.Landing, IsStateTimeout },
        //     };
        //
        //     _stateChangeTable = new()
        //     {
        //         { JumpState.None, state => state },
        //         { 
        //             JumpState.StartJump, _ =>  new JumpCurrentState
        //         {
        //             CurrentState = JumpState.InAir, CurrentStateTime = _timeInAir, StartHeight = _characterController.height
        //         } },
        //         { 
        //             JumpState.InAir, _ => new JumpCurrentState
        //         {
        //             CurrentState = JumpState.Landing, CurrentStateTime = _fallTime, StartHeight = _characterController.height
        //         } },
        //         { JumpState.Landing, state => state },
        //     };
        //     
        //     static bool IsStateTimeout(JumpCurrentState state)
        //     {
        //         return state.CurrentStateTime <= 0.0f;
        //     }
        // }

        private void Start()
        {
           // Messenger.Default.Subscribe<PlayerInputPayload>(OnPlayerInput);
        }
        
        private void OnPlayerInput(PlayerInputPayload payload)
        {
          //  if (!payload.IsSameCharacter(_idHolder)) return;
            
            if (payload.PlayerInput.IsJumpPressed && _curState.CurrentState == JumpState.None)
                InitJump();
        }

        protected override void UpdateAction()
        {
            
            var input = _playerInput.PlayerInput;
            
            if (input.IsJumpPressed && _curState.CurrentState == JumpState.None)
                InitJump();
            
            if (_curState.CurrentState == JumpState.None)
                return;
            
            _curState = _curState.SetStateTime(_curState.CurrentStateTime - Time.deltaTime);

            if (_curState.CurrentStateTime <= 0.0f)
            {
                TransitionState();
                return;
            }
            
            UpdateState();
        }
        
        
#region State Transition
        static Dictionary<JumpState, JumpState> stateTransitionTable = new()
        {
            { JumpState.None, JumpState.None },
            { JumpState.StartJump, JumpState.InAir },
            { JumpState.InAir, JumpState.Landing },
            { JumpState.Landing, JumpState.None },
        };
        private void TransitionState()
        {
            if (!stateTransitionTable.TryGetValue(_curState.CurrentState, out var nextState))
                return;
            
            switch (nextState)
            {
                case JumpState.InAir:
                    SetInAir();
                    break;
                case JumpState.Landing:
                    SetLanding();
                    break;
                case JumpState.None:
                    ClearJumpState();
                    break;
                case JumpState.StartJump:
                    break;
            }
        }
        private void SetInAir() => _curState = _curState.SetCurrentState(JumpState.InAir).SetStateTime(_timeInAir);
        private void SetLanding()
        {
            _curState = _curState.SetCurrentState(JumpState.Landing).SetStateTime(_fallTime);
            _animationHandler.PlayAnimation(_fallKey);
        }

        #endregion
        
#region State Update
        private void UpdateState()
        {
            switch (_curState.CurrentState)
            {
                case JumpState.None:
                    break;
                case JumpState.StartJump:
                    UpdateStartJump();
                    break;
                case JumpState.InAir:
                    UpdateInAir();
                    break;
                case JumpState.Landing:
                    UpdateLanding();
                    break;
            }
        }
        private void UpdateStartJump()
        {
            var motion = SampleCurveMotion(_jumpCurve, _jumpHeight);
            _characterController.Move(motion * Vector3.up);
        }
        private void UpdateInAir(){}
        private void UpdateLanding()
        {
            var motion = SampleCurveMotion(_fallCurve, _jumpHeight);
            _characterController.Move(motion * Vector3.down);
        }
        private float SampleCurveMotion(AnimationCurve curve, float travelDistance)
        {
            //sample jump curve
            var sampleTime = _jumpTime - _curState.CurrentStateTime;
            var curProgress = curve.Evaluate((sampleTime - Time.deltaTime) / _jumpTime);
            var jumpProgress = curve.Evaluate(sampleTime / _jumpTime);
            var jumpDist = travelDistance * (jumpProgress - curProgress);
            
            return jumpDist;
        }
#endregion


        private void InitJump()
        {
            ResetJumpState();
            
            _curState = _curState.SetStartHeight(_characterController.height)
                .SetCurrentState(JumpState.StartJump)
                .SetStateTime(_jumpTime);
            
            Messenger.Default.Publish(new CharacterStateUpdateRequestPayload(StateModificationType.Add, CharacterState.Jump,
                _idHolder.CharacterId));
            
            _animationHandler.PlayAnimation(_jumpKey);
            
        }
        private void ResetJumpState()
        {
            _curState = _curState.SetDefault();
        }
        protected override void OnCancelAction()
        {
            base.OnCancelAction();
            ResetJumpState();
            Messenger.Default.Publish(new CharacterStateUpdateRequestPayload(StateModificationType.Remove, CharacterState.Jump,
                _idHolder.CharacterId));
        }
        
        private void ClearJumpState()
        {
            _curState = _curState.SetDefault();
            Messenger.Default.Publish(new CharacterStateUpdateRequestPayload(StateModificationType.Remove, CharacterState.Jump,
                _idHolder.CharacterId));
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerInputPayload>(OnPlayerInput);
        }
    }
    
}
using System;
using FGSTest.Common;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class AccelerateAction : CharacterAction
    {
        [Header("Accelerate Config")]
        [SerializeField] private float _spedUpAmount;
        [SerializeField] private float _speedUpDuration;
        [SerializeField] private float _timeoutDuration;

        [SerializeField] private AccelerateCurrentState _curState;
        [SerializeField] private PlayerInputReceiver _playerInput;

        [Serializable]
        public enum AccelerateState
        {
            TimeOut = 0,
            Ready = 1,
            StartAccelerate = 2, //pending for now
            Accelerated = 3,
        }
        
        [Serializable]
        public struct AccelerateCurrentState
        {
            public AccelerateState CurrentState;
            public float CurrentTime;
            
            public AccelerateCurrentState SetCurrentTime(float time)
            {
                CurrentTime = time;
                return this;
            }

            public AccelerateCurrentState SetCurrentState(AccelerateState state)
            {
                CurrentState = state;
                return this;
            }
        }

        private AccelerateCurrentState SetTimeOut()
        {
            return new AccelerateCurrentState().SetCurrentState(AccelerateState.TimeOut)
                .SetCurrentTime(_timeoutDuration);
        }
        

        private void ClearAccelerate()
        {
            _curState = SetTimeOut();
            
            Messenger.Default.Publish(
                new CharacterStatUpdateRequestPayload(CharacterStatType.Speed, -_spedUpAmount));
            
            Messenger.Default.Publish(
                new CharacterStateUpdateRequestPayload(StateModificationType.Remove, 
                    CharacterState.Accelerate,
                    _idHolder.CharacterId));
        }

        protected override void UpdateAction()
        {
            if (_playerInput.PlayerInput.IsAccelerationPressed &&
                _curState.CurrentState == AccelerateState.Ready)
            {
                ActivateAccelerate();
                return;
            }
            
            if (_curState.CurrentState == AccelerateState.Ready)
                return;
            
            _curState = _curState.SetCurrentTime(_curState.CurrentTime - Time.deltaTime);

            if (_curState.CurrentTime <= 0.0f)
                TransitionState();
        }

        private void TransitionState()
        {
            if (_curState.CurrentState == AccelerateState.Accelerated)
            {
                ClearAccelerate();
                return;
            }

            if (_curState.CurrentState == AccelerateState.TimeOut)
            {
                _curState = _curState.SetCurrentState(AccelerateState.Ready);
                return;
            }
        }
        
        private void ActivateAccelerate()
        {
            Messenger.Default.Publish(
                new CharacterStatUpdateRequestPayload(CharacterStatType.Speed, _spedUpAmount));

            Messenger.Default.Publish(
                new CharacterStateUpdateRequestPayload(StateModificationType.Add, 
                    CharacterState.Accelerate,
                    _idHolder.CharacterId));

            _curState = _curState.SetCurrentState(AccelerateState.Accelerated).SetCurrentTime(_speedUpDuration);
        }

        protected override void OnCancelAction()
        {
            base.OnCancelAction();
            ClearAccelerate();
        }
        
    }
}
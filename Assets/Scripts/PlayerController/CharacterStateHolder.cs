using System;
using System.Collections.Generic;
using FGSTest.Common;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using Unity.Collections;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class CharacterStateHolder : MonoBehaviour, IGetCharacterState
    {
        [SerializeField] private CharacterIdentityHolder _idHolder;
        [SerializeField, ReadOnly] private CharacterState _characterState;
        public CharacterState CharacterState => _characterState;
        
        [SerializeReference] List<IStateUpdater> _stateUpdaters;

        private void Awake()
        {
            //on demand state change
            Messenger.Default.Subscribe<CharacterStateUpdateRequestPayload>(OnCharacterStateUpdateRequest);
        }

        private void Start()
        {
            Messenger.Default.Publish(new CharacterStateUpdatePayload(_characterState, _idHolder.CharacterId));
        }

        //frequent state update
        private void Update()
        {
            var cacheState = _characterState;
            
            foreach (var stateUpdater in _stateUpdaters)
            {
                _characterState = (_characterState & ~stateUpdater.Mask) | (stateUpdater.CharacterState & stateUpdater.Mask);
            }
            
            DefaultToIdle();
            
            if (cacheState != _characterState)
                Messenger.Default.Publish(new CharacterStateUpdatePayload(_characterState, _idHolder.CharacterId));
        }

        private void DefaultToIdle()
        {
            if (_characterState == CharacterState.None)
                _characterState = CharacterState.Idle;
        }

        private void OnCharacterStateUpdateRequest(CharacterStateUpdateRequestPayload payload)
        {
            if (!payload.IsSameCharacter(_idHolder)) return;
            
            if (!ValidCheckState(payload.ModificationType, payload.CharacterState)) return;
            
            var cacheState = _characterState;

            if (payload.ModificationType == StateModificationType.Add)
            {
                _characterState |= payload.CharacterState;
            }
            else if (payload.ModificationType == StateModificationType.Remove)
            {
                _characterState &= ~payload.CharacterState;
            }
            
            ClearNonGroundedState();
            
            DefaultToIdle();
            
            if (cacheState != _characterState)
                Messenger.Default.Publish(new CharacterStateUpdatePayload(_characterState, _idHolder.CharacterId));
            
        }
        
        private void ClearNonGroundedState()
        {
            if (_characterState.HasFlag(CharacterState.Grounded))
                return;
            
            _characterState = _characterState & ~CharacterState.Walk;
            _characterState = _characterState & ~CharacterState.Idle;
        }

        private bool ValidCheckState(StateModificationType modType, CharacterState state)
        {
            if (modType == StateModificationType.Add)
            {
                if ((state == CharacterState.Walk) && !_characterState.HasFlag(CharacterState.Grounded))
                    return false;
            }

            return true;
        }

        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<CharacterStateUpdateRequestPayload>(OnCharacterStateUpdateRequest);
        }
    }
    
    
    public abstract class IStateUpdater : MonoBehaviour
    {
        public abstract CharacterState Mask { get; }
        public abstract CharacterState CharacterState { get; }
    }
}
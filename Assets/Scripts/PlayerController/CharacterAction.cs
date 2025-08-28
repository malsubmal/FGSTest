using System;
using FGSTest.Common;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public abstract class CharacterAction : MonoBehaviour
    {
        [Header("Action Setting")]
        [SerializeField] protected CharacterIdentityHolder _idHolder;
        [SerializeField] private CharacterState _cancelActionFlag;
        [SerializeField] private CharacterState _ignoreActionFlag;
        [SerializeField] private CharacterState _actionFlag;

        [SerializeField] private CharacterStateHolder _characterState;
        private bool _hasIgnoredFlag;
        private bool _hasActionFlag;

        private void Awake()
        {
            Messenger.Default.Subscribe<CharacterStateUpdatePayload>(OnCharacterStateUpdate);
            
            _hasIgnoredFlag = (_characterState.CharacterState & _ignoreActionFlag) != 0;
            _hasActionFlag = (_characterState.CharacterState & _actionFlag) != 0;
        }
        
        private void OnCharacterStateUpdate(CharacterStateUpdatePayload payload)
        {
          //  if (!payload.IsSameCharacter(_idHolder)) return;
            
            _hasIgnoredFlag = (payload.CharacterState & _ignoreActionFlag) != 0;
            _hasActionFlag = (payload.CharacterState & _actionFlag) != 0;
            
            if ((payload.CharacterState & _cancelActionFlag) != 0)
            {
                CancelAction();
            }
        }

        private void Update()
        {
            //must have flag
            if (_hasIgnoredFlag)
                return;
            
            UpdateAction();
        }

        protected abstract void UpdateAction();

        private void CancelAction()
        {
            //if (_characterState.CharacterState.HasFlag(_actionFlag))
              //  Messenger.Default.Publish(new CharacterStateUpdateRequestPayload(StateModificationType.Remove, _actionFlag, _idHolder.CharacterId));
            OnCancelAction();
        }

        protected virtual void OnCancelAction()
        {
            
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<CharacterStateUpdatePayload>(OnCharacterStateUpdate);
            DestroyAction();
        }

        protected virtual void DestroyAction()
        {
            
        }
            
    }



}
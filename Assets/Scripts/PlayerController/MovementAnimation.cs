using FGSTest.Common;
using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class MovementAnimation : MonoBehaviour
    {
        [SerializeField] private AnimationKey _walkKey;
        [SerializeField] private AnimationKey _runKey;
        [SerializeField] private AnimationKey _idleKey;
    
        [SerializeField] private CharacterAnimationHandler _animationHandler;

        private void Awake()
        {
            Messenger.Default.Subscribe<CharacterStateUpdatePayload>(OnCharacterStateUpdate);
        }

        private void OnCharacterStateUpdate(CharacterStateUpdatePayload payload)
        {
            if (payload.CharacterState.HasFlag(CharacterState.Idle))
                _animationHandler.PlayAnimation(_idleKey);
            
            if (payload.CharacterState.HasFlag(CharacterState.Walk))
                _animationHandler.PlayAnimation(_walkKey);
            
            var runFlag = CharacterState.Walk | CharacterState.Accelerate;
            if (payload.CharacterState.HasFlag(runFlag))
                _animationHandler.PlayAnimation(_runKey);
            
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<CharacterStateUpdatePayload>(OnCharacterStateUpdate);
        }
    }
}
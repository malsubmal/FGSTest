using System;

namespace FGSTest.Payload
{
    [Flags]
    public enum CharacterState
    {
        None = 0,
        Idle = 1 << 7,
        Walk = 1 << 0,
        //Run = 1 << 1,
        Accelerate = 1 << 2,
        Jump = 1 << 3,
        Dead = 1 << 4,
        Spawning = 1 << 5,
        Grounded = 1 << 6,
    }
    
    
    [Serializable]
    public struct CharacterStateUpdatePayload : ICharacterIdentity
    {
        public CharacterStateUpdatePayload(CharacterState characterState, CharacterIdentity characterIdentity)
        {
            _characterState = characterState;
            _characterIdentity = characterIdentity;
        }

        private CharacterState _characterState;
        private CharacterIdentity _characterIdentity;
        
        public CharacterState CharacterState => _characterState;
        public CharacterIdentity CharacterId => _characterIdentity;
    }
    
    [Serializable]
    public struct CharacterStateUpdateRequestPayload : ICharacterIdentity
    {
        public CharacterStateUpdateRequestPayload(StateModificationType modType, CharacterState characterState, CharacterIdentity characterIdentity)
        {
            _modType = modType;
            _characterState = characterState;
            _characterIdentity = characterIdentity;
        }

        private StateModificationType _modType;
        private CharacterState _characterState;
        private CharacterIdentity _characterIdentity;
        
        public CharacterState CharacterState => _characterState;
        public CharacterIdentity CharacterId => _characterIdentity;
        public StateModificationType ModificationType => _modType;
    }
    
    public interface IGetCharacterState
    {
        public CharacterState CharacterState { get; }
    }
    
    [Serializable]
    public enum StateModificationType : byte
    {
        Add = 1,
        Remove = 2,
    }
}
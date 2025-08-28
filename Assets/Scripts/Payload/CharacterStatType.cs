using System;

namespace FGSTest.Payload
{
    [Serializable]
    public enum CharacterStatType : int
    {
        None = 0,
        Speed = 1,
    }
    
    [Serializable]
    public struct CharacterStatUpdateRequestPayload
    {
        public CharacterStatType StatType;
        public float ChangeValue;
        
        public CharacterStatUpdateRequestPayload(CharacterStatType statType, float changeValue)
        {
            StatType = statType;
            ChangeValue = changeValue;
        }
    }
    
    [Serializable]
    public struct CharacterStatUpdatePayload
    {
        public CharacterStatType StatType;
        public float Value;
        
        public CharacterStatUpdatePayload(CharacterStatType statType, float value)
        {
            StatType = statType;
            Value = value;
        }
    }
}
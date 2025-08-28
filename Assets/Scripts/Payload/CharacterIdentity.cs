using System;
using UnityEngine;

namespace FGSTest.Payload
{
    [Serializable]
    public struct CharacterIdentity : IEquatable<CharacterIdentity>
    {
        [SerializeField] private int _id;
        
        public bool Equals(CharacterIdentity other)
        {
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            return obj is CharacterIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _id;
        }
    }
    
    public interface ICharacterIdentity
    {
        public CharacterIdentity CharacterId { get; }
    }
    
    public static class CharacterIdentityExtensions
    {
        public static bool IsSameCharacter<T, V>(this T self, V other)
            where T : ICharacterIdentity
            where V : ICharacterIdentity
            => self.CharacterId.Equals(other.CharacterId);
    }
}
using FGSTest.Payload;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class CharacterIdentityHolder : MonoBehaviour, ICharacterIdentity
    {
        [SerializeField] protected CharacterIdentity _characterIdentity;
        public CharacterIdentity CharacterId => _characterIdentity;
    }
}
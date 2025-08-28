using FGSTest.Payload;
using SuperMaxim.Messaging;
using UnityEngine;

namespace FGSTest.PlayerController
{
    public class PlayerSpawnNotifier : MonoBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Transform _playerTransform;
        private void Start()
        {
            Messenger.Default.Publish(new PlayerSpawnPayload(_playerTransform, _characterController.radius, _characterController.GetInstanceID()));
        }
    }
}
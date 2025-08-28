using UnityEngine;

namespace FGSTest.PlayerController
{
    public class CharacterFacingDirection : MonoBehaviour
    {
        [SerializeField] private Transform _visualTransform;
        [SerializeField] private PlayerInputReceiver _playerInput;

        private void Update()
        {
            if (_playerInput.PlayerInput.Movement == Vector2.zero)
                return;
            
            var moveDir = _playerInput.PlayerInput.Movement;
            var moveDirXZ = new Vector3(moveDir.x, 0, moveDir.y);
            
            _visualTransform.localEulerAngles = new Vector3(0, Mathf.Atan2(moveDirXZ.x, moveDirXZ.z) * Mathf.Rad2Deg, 0);
        }
    }
}
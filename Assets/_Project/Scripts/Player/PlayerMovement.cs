using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Value / Vector2 action bound to a WASD Up/Down/Left/Right composite.")]
        [SerializeField] private UnityEngine.InputSystem.InputActionReference moveAction;

        private CharacterController _controller;
        // NOTE: Vertical speed from gravity only, kept apart from horizontal input.
        private float _verticalVelocity;

        private void Awake() => _controller = GetComponent<CharacterController>();

        private void OnEnable()
        {
            if (!moveAction.TryEnable(this))
                enabled = false;
        }

        private void Update()
        {
            GameConfig.PlayerMovementConfig config = GameConfig.Movement;

            Vector2 input = Vector2.ClampMagnitude(moveAction.action.ReadValue<Vector2>(), 1f);
            // NOTE: Uses the body's yaw only (pitch lives on the camera), so looking up/down never changes walk direction.
            Vector3 horizontal = (transform.right * input.x + transform.forward * input.y) * config.MoveSpeed;

            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = config.GroundedStickVelocity;
            _verticalVelocity += config.Gravity * Time.deltaTime;

            // NOTE: Exactly one Move() per frame – calling it twice makes isGrounded unreliable.
            _controller.Move((horizontal + Vector3.up * _verticalVelocity) * Time.deltaTime);
        }
    }
}
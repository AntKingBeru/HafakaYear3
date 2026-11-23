using Hafaka.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hafaka.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [Tooltip("Value / Vector2 action bound to <Mouse>/delta.")]
        [SerializeField] private InputActionReference lookAction;
        [Tooltip("Camera transform (child of the player). Pitch is applied to it; yaw is applied to this object.")]
        [SerializeField] private Transform cameraTransform;

        private float _pitch; // NOTE: Tracked as a float so clamping never breaks on Euler wrap-around (359°).
        private bool _skipNextDelta; // NOTE: Drops the first delta after re-enabling – the cursor re-lock can cause a jump.

        private void Awake()
        {
            if (cameraTransform)
                _pitch = NormalizeAngle(cameraTransform.localEulerAngles.x);
        }

        private void OnEnable()
        {
            if (!cameraTransform)
            {
                Debug.LogError("[PlayerLook] Camera Transform is not assigned.", this);
                enabled = false;
                return;
            }

            if (!lookAction.TryEnable(this))
            {
                enabled = false;
                return;
            }
            
            _skipNextDelta = true;
        }

        private void Update()
        {
            Vector2 delta = lookAction.action.ReadValue<Vector2>();
            if (_skipNextDelta)
            {
                _skipNextDelta = false;
                return;
            }

            GameConfig.PlayerLookConfig config = GameConfig.Look;
            delta *= config.Sensitivity;

            transform.Rotate(0f, delta.x, 0f, Space.Self);

            float pitchDelta = config.InvertY ? delta.y : -delta.y;
            _pitch = Mathf.Clamp(_pitch + pitchDelta, config.MinPitch, config.MaxPitch);
            cameraTransform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private static float NormalizeAngle(float angle) => angle > 180f ? angle - 360f : angle;
    }
}
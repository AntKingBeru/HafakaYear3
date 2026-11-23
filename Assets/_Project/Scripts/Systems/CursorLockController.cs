using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Systems
{
    public class CursorLockController : MonoBehaviour
    {
        // NOTE: Desired state. Kept separately because the OS/editor can unlock the cursor behind our back (alt-tab).
        private bool _shouldLock;

        private void OnEnable()
        {
            GameEventsRelay relay = GameEventsRelay.Instance;
            if (!relay)
            {
                Debug.LogError("[CursorLockController] GameEventsRelay missing.", this);
                return;
            }
            
            relay.OnPauseChanged.AddListener(HandlePauseChanged);
        }

        private void OnDisable()
        {
            GameEventsRelay relay = GameEventsRelay.Instance;
            if (relay)
                relay.OnPauseChanged.RemoveListener(HandlePauseChanged);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                ApplyCursorState();
        }

        private void HandlePauseChanged(bool isPaused)
        {
            _shouldLock = !isPaused;
            ApplyCursorState();
        }

        private void ApplyCursorState()
        {
            Cursor.lockState = _shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !_shouldLock;
        }
    }
}
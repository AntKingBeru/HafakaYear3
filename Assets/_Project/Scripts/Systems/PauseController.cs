using Hafaka.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hafaka.Systems
{
    public class PauseController : MonoBehaviour
    {
        [Tooltip("Button action bound to <Keyboard>/escape.")]
        [SerializeField] private InputActionReference pauseAction;

        public bool IsPaused { get; private set; }

        private void OnEnable()
        {
            if (!pauseAction.TryEnable(this))
            {
                enabled = false;
                return;
            }
            
            pauseAction.action.performed += OnPausePerformed;
        }

        private void OnDisable()
        {
            if (pauseAction.IsValid())
                pauseAction.action.performed -= OnPausePerformed;
        }

        // NOTE: Broadcasts the initial state so every listener (cursor, player gate) starts in sync.
        private void Start() => SetPaused(false);

        private void OnPausePerformed(InputAction.CallbackContext _) => SetPaused(!IsPaused);

        // NOTE: Public so the future UI Toolkit "Resume" button can call SetPaused(false).
        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            GameEventsRelay relay = GameEventsRelay.Instance;
            if (relay)
                relay.OnPauseChanged.Invoke(paused);
        }
    }
}
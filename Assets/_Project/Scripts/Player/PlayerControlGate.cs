using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Player
{
    public class PlayerControlGate : MonoBehaviour
    {
        // NOTE: Assign PlayerMovement, PlayerLook, InteractionTargetFinder, PlayerInteractor.
        [Tooltip("Behaviours switched off while the game is paused.")]
        [SerializeField] private Behaviour[] gatedBehaviours;

        private void OnEnable()
        {
            GameEventsRelay relay = GameEventsRelay.Instance;
            if (!relay)
            {
                Debug.LogError("[PlayerControlGate] GameEventsRelay missing.", this);
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

        private void HandlePauseChanged(bool isPaused)
        {
            foreach (var behaviour in gatedBehaviours)
            {
                if (behaviour && behaviour != this)
                    behaviour.enabled = !isPaused;
            }
        }
    }
}
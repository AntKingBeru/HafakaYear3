using Hafaka.Interactions;
using UnityEngine;
using UnityEngine.Events;

namespace Hafaka.Core
{
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public class GameEventsRelay : MonoBehaviour
    {
        // NOTE: Singleton. Listeners subscribe in OnEnable and must null-check Instance in OnDisable (teardown order).
        public static GameEventsRelay Instance { get; private set; }

        #region Game State
        // NOTE: Raised by PauseController. true = paused (cursor free, player frozen).
        [field: SerializeField] public UnityEvent<bool> OnPauseChanged { get; private set; } = new();
        #endregion

        #region Interaction
        // NOTE: Raised by InteractionTargetFinder when the looked-at target changes. Null = nothing targeted. (Future crosshair/prompt UI.)
        [field: SerializeField] public UnityEvent<Interactable> OnInteractionTargetChanged { get; private set; } = new();

        // NOTE: Raised by ItemHolder after an item is in the player's hands.
        [field: SerializeField] public UnityEvent<PickupItem> OnItemPickedUp { get; private set; } = new();

        // NOTE: Raised by PlacementSpot after an item is placed on it.
        [field: SerializeField] public UnityEvent<PickupItem, PlacementSpot> OnItemPlaced { get; private set; } = new();
        #endregion

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Debug.LogWarning("[GameEventsRelay] Duplicate relay found – removing it.", this);
                Destroy(this);
                return;
            }
            
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
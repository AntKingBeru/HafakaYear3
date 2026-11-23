using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Interactions
{
    public class PlacementSpotIndicator : MonoBehaviour
    {
        [SerializeField] private PlacementSpot spot;
        [Tooltip("Visual marker child. Must NOT be this GameObject, must not contain the Anchor, and should have no collider.")]
        [SerializeField] private GameObject marker;

        private void Reset() => spot = GetComponent<PlacementSpot>();

        private void OnEnable()
        {
            SetMarkerVisible(false);
            GameEventsRelay relay = GameEventsRelay.Instance;
            if (!relay)
            {
                Debug.LogError("[PlacementSpotIndicator] GameEventsRelay missing.", this);
                return;
            }
            
            relay.OnItemPickedUp.AddListener(HandleItemPickedUp);
            relay.OnItemPlaced.AddListener(HandleItemPlaced);
        }

        private void OnDisable()
        {
            GameEventsRelay relay = GameEventsRelay.Instance;
            if (!relay)
                return;
            relay.OnItemPickedUp.RemoveListener(HandleItemPickedUp);
            relay.OnItemPlaced.RemoveListener(HandleItemPlaced);
        }

        private void HandleItemPickedUp(PickupItem item) => SetMarkerVisible(spot && spot.CanAccept(item));

        // NOTE: After any placement the hands are empty, so every marker hides.
        private void HandleItemPlaced(PickupItem item, PlacementSpot placedOn) => SetMarkerVisible(false);

        private void SetMarkerVisible(bool isVisible)
        {
            if (marker && marker != gameObject)
                marker.SetActive(isVisible);
        }
    }
}
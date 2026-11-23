using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Interactions
{
    public class ItemHolder : MonoBehaviour
    {
        [Tooltip("Child of the camera where held items are carried.")]
        [SerializeField] private Transform holdPoint;

        public PickupItem CurrentItem { get; private set; }
        public bool IsHolding => CurrentItem;

        private void Awake()
        {
            if (!holdPoint)
                Debug.LogError("[ItemHolder] Hold Point is not assigned.", this);
        }

        // NOTE: Only called after CanInteract confirmed the hands are empty. Raises OnItemPickedUp.
        public void Hold(PickupItem item)
        {
            if (!item || IsHolding || !holdPoint)
            {
                Debug.LogWarning("[ItemHolder] Cannot hold item (null item, hands full, or no hold point).", this);
                return;
            }

            CurrentItem = item;
            item.AttachTo(holdPoint);

            GameEventsRelay relay = GameEventsRelay.Instance;
            if (relay)
                relay.OnItemPickedUp.Invoke(item);
        }

        // NOTE: Hands the item over (e.g. to a PlacementSpot). Doesn't move it – the receiver decides where it goes.
        public PickupItem Release()
        {
            PickupItem item = CurrentItem;
            CurrentItem = null;
            return item;
        }
    }
}
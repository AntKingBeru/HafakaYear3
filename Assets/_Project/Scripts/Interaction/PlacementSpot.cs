using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Interactions
{
    [DisallowMultipleComponent]
    public class PlacementSpot : Interactable
    {
        [Tooltip("Where the placed item's pivot snaps to. Must NOT be under the marker. Defaults to this transform.")]
        [SerializeField] private Transform anchor;
        [Tooltip("Collider the interaction ray hits (trigger recommended). Defaults to the collider on this object.")]
        [SerializeField] private Collider targetCollider;

        public PickupItem Occupant { get; private set; }
        public bool IsOccupied => Occupant;
        public Transform Anchor => anchor ? anchor : transform;

        private void Awake()
        {
            if (!targetCollider)
                targetCollider = GetComponent<Collider>();
            if (!targetCollider)
                Debug.LogError("[PlacementSpot] Needs a collider for the interaction ray to hit.", this);
        }

        // NOTE: Extension point – later this can filter which items a spot accepts.
        public virtual bool CanAccept(PickupItem item) => item && !IsOccupied;

        public override bool CanInteract(ItemHolder holder) =>
            holder && holder.IsHolding && CanAccept(holder.CurrentItem);

        public override void Interact(ItemHolder holder)
        {
            if (!CanInteract(holder))
                return;

            PickupItem item = holder.Release();
            Occupant = item;
            item.PlaceOn(this);

            // NOTE: Occupied spots drop out of the ray, so the item sitting on them can be picked up again.
            SetTargetable(false);
            SetFocused(false);

            GameEventsRelay relay = GameEventsRelay.Instance;
            if (relay)
                relay.OnItemPlaced.Invoke(item, this);
        }

        // NOTE: Called by PickupItem when it's picked up from this spot.
        internal void Vacate(PickupItem item)
        {
            if (Occupant != item)
                return;
            Occupant = null;
            SetTargetable(true);
        }

        private void SetTargetable(bool isTargetable)
        {
            if (targetCollider)
                targetCollider.enabled = isTargetable;
        }
    }
}
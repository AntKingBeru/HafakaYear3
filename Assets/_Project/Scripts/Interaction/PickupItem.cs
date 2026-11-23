using UnityEngine;

namespace Hafaka.Interactions
{
    [DisallowMultipleComponent]
    public class PickupItem : Interactable
    {
        private Collider[] _colliders;
        private bool[] _colliderDefaults; // NOTE: Original enabled state per collider, restored on placement.
        private Rigidbody _rigidbody; // NOTE: Optional. Becomes kinematic on first pickup and stays that way.

        // NOTE: The spot this item currently sits on (null if loose or held).
        public PlacementSpot CurrentSpot { get; private set; }

        private void Awake()
        {
            _colliders = GetComponentsInChildren<Collider>(true);
            _colliderDefaults = new bool[_colliders.Length];
            for (var i = 0; i < _colliders.Length; i++)
                _colliderDefaults[i] = _colliders[i].enabled;

            _rigidbody = GetComponent<Rigidbody>();
        }

        public override bool CanInteract(ItemHolder holder) => holder && !holder.IsHolding;

        public override void Interact(ItemHolder holder)
        {
            if (!CanInteract(holder))
                return;

            // NOTE: Free the spot BEFORE holding, so OnItemPickedUp listeners already see it as available.
            if (CurrentSpot)
            {
                PlacementSpot previousSpot = CurrentSpot;
                CurrentSpot = null;
                previousSpot.Vacate(this);
            }

            SetFocused(false);
            holder.Hold(this);
        }

        // NOTE: Called by ItemHolder. Collisions off so the item never pushes the player or blocks the interaction ray.
        internal void AttachTo(Transform holdPoint)
        {
            FreezePhysics();
            SetCollidersEnabled(false);
            SnapTo(holdPoint);
        }

        // NOTE: Called by PlacementSpot.
        internal void PlaceOn(PlacementSpot spot)
        {
            CurrentSpot = spot;
            SnapTo(spot.Anchor);
            RestoreColliders();
        }

        private void SnapTo(Transform parent)
        {
            transform.SetParent(parent, true); // NOTE: worldPositionStays = true keeps world scale under scaled parents.
            transform.SetPositionAndRotation(parent.position, parent.rotation);
        }

        private void FreezePhysics()
        {
            if (!_rigidbody || _rigidbody.isKinematic)
                return;
            // NOTE: Zero velocity while still dynamic – Unity 6 warns if you set it on a kinematic body.
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _rigidbody.interpolation = RigidbodyInterpolation.None; // NOTE: Avoids jitter while parented to the camera.
            _rigidbody.isKinematic = true;
        }

        private void SetCollidersEnabled(bool isEnabled)
        {
            foreach (var col in _colliders)
                if (col)
                    col.enabled = isEnabled;
        }

        private void RestoreColliders()
        {
            for (var i = 0; i < _colliders.Length; i++)
                if (_colliders[i])
                    _colliders[i].enabled = _colliderDefaults[i];
        }
    }
}
using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Interactions
{
    public class InteractionTargetFinder : MonoBehaviour
    {
        private const int MaxHits = 8; // NOTE: Raycast buffer size; hits beyond this are ignored.

        [Tooltip("Usually the camera. The ray starts here and goes forward.")]
        [SerializeField] private Transform rayOrigin;
        [SerializeField] private ItemHolder holder;

        private readonly RaycastHit[] _hits = new RaycastHit[MaxHits];

        // NOTE: What PlayerInteractor acts on when E is pressed. Null = nothing targeted.
        public Interactable CurrentTarget { get; private set; }

        private void Reset()
        {
            holder = GetComponent<ItemHolder>();
            Camera cam = GetComponentInChildren<Camera>();
            if (cam)
                rayOrigin = cam.transform;
        }

        private void Update() => SetTarget(FindTarget());

        // NOTE: Clears the highlight when paused (PlayerControlGate disables this component).
        private void OnDisable() => SetTarget(null);

        // NOTE: Nearest valid Interactable that isn't behind a solid non-interactable collider. Triggers never block.
        private Interactable FindTarget()
        {
            if (!rayOrigin)
                return null;

            GameConfig.InteractionConfig config = GameConfig.Interaction;
            int count = Physics.RaycastNonAlloc(rayOrigin.position, rayOrigin.forward, _hits,
                config.Range, config.InteractionMask, QueryTriggerInteraction.Collide);

            Interactable best = null;
            float bestDistance = float.MaxValue;
            float blockDistance = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                RaycastHit hit = _hits[i];
                Interactable candidate = hit.collider.GetComponentInParent<Interactable>();

                if (candidate && candidate.CanInteract(holder))
                {
                    if (hit.distance < bestDistance)
                    {
                        best = candidate;
                        bestDistance = hit.distance;
                    }
                }
                else if (!hit.collider.isTrigger && hit.distance < blockDistance)
                {
                    blockDistance = hit.distance;
                }
            }

            return bestDistance <= blockDistance ? best : null;
        }

        private void SetTarget(Interactable newTarget)
        {
            if (newTarget == CurrentTarget)
                return;

            if (CurrentTarget)
                CurrentTarget.SetFocused(false);
            CurrentTarget = newTarget;
            if (CurrentTarget)
                CurrentTarget.SetFocused(true);

            GameEventsRelay relay = GameEventsRelay.Instance;
            if (relay)
                relay.OnInteractionTargetChanged.Invoke(CurrentTarget);
        }
    }
}
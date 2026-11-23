using Hafaka.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hafaka.Interactions
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Tooltip("Button action bound to <Keyboard>/e (no Hold interaction).")]
        [SerializeField] private InputActionReference interactAction;
        [SerializeField] private InteractionTargetFinder targetFinder;
        [SerializeField] private ItemHolder holder;

        private void Reset()
        {
            targetFinder = GetComponent<InteractionTargetFinder>();
            holder = GetComponent<ItemHolder>();
        }

        private void OnEnable()
        {
            if (!interactAction.TryEnable(this))
            {
                enabled = false;
                return;
            }
            
            interactAction.action.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            if (interactAction.IsValid())
                interactAction.action.performed -= OnInteractPerformed;
        }

        // NOTE: Interact() re-checks CanInteract itself, so a target that changed since last frame is handled safely.
        private void OnInteractPerformed(InputAction.CallbackContext _)
        {
            if (!targetFinder)
                return;
            Interactable target = targetFinder.CurrentTarget;
            if (target)
                target.Interact(holder);
        }
    }
}
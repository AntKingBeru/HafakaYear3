using Hafaka.Interactions.Highlighting;
using UnityEngine;

namespace Hafaka.Interactions
{
    public abstract class Interactable : MonoBehaviour
    {
        [Tooltip("Optional visual shown while this is the player's current target.")]
        [SerializeField] private HighlightBase highlight;

        // NOTE: Asked every frame by InteractionTargetFinder. Return false to make this un-targetable right now.
        public abstract bool CanInteract(ItemHolder holder);

        // NOTE: Called by PlayerInteractor when Interact is pressed while this is the current target.
        public abstract void Interact(ItemHolder holder);

        public void SetFocused(bool isFocused)
        {
            if (highlight)
                highlight.SetHighlighted(isFocused);
        }

        protected virtual void Reset() => highlight = GetComponentInChildren<HighlightBase>();
    }
}
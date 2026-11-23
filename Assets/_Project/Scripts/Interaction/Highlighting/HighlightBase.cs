using UnityEngine;

namespace Hafaka.Interactions.Highlighting
{
    public abstract class HighlightBase : MonoBehaviour
    {
        // NOTE: Must be safe to call repeatedly with the same value, and before Awake (object may be inactive).
        public abstract void SetHighlighted(bool isHighlighted);
    }
}
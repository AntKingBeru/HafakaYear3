using UnityEngine;
using UnityEngine.InputSystem;

namespace Hafaka.Core
{
    public static class InputActionReferenceExtensions
    {
        // NOTE: Logs and returns false if unassigned, so callers can disable themselves instead of throwing every frame.
        public static bool TryEnable(this InputActionReference reference, Object context)
        {
            if (!reference.IsValid())
            {
                Debug.LogError($"[{context.GetType().Name}] InputActionReference is not assigned.", context);
                return false;
            }
            
            reference.action.Enable();
            return true;
        }

        public static bool IsValid(this InputActionReference reference) => reference && reference.action != null;
    }
}
using System;
using UnityEngine;

namespace Hafaka.Core
{
    [DefaultExecutionOrder(-101)]
    [DisallowMultipleComponent]
    public class GameConfig : MonoBehaviour
    {
        // NOTE: Singleton instance, assigned in Awake. Read config through the static accessors below.
        public static GameConfig Instance { get; private set; }

        [SerializeField] private PlayerMovementConfig movement = new();
        [SerializeField] private PlayerLookConfig look = new();
        [SerializeField] private InteractionConfig interaction = new ();
        [SerializeField] private HighlightConfig highlight = new();
        
        // NOTE: Static accessors – usage: GameConfig.Movement.MoveSpeed
        public static PlayerMovementConfig Movement => Current.movement;
        public static PlayerLookConfig Look => Current.look;
        public static InteractionConfig Interaction => Current.interaction;
        public static HighlightConfig Highlight => Current.highlight;

        private static GameConfig Current
        {
            get
            {
                if (!Instance)
                    Debug.LogError("[GameConfig] No GameConfig in the scene. Add it to the Systems object.");
                return Instance;
            }
        }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Debug.LogWarning("[GameConfig] Duplicate GameConfig found – removing it.", this);
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
        
        // NOTE: Which color a TintHighlight uses from HighlightConfig.
        public enum HighlightStyle
        {
            Pickup,
            PlacementSpot
        }

        [Serializable]
        public class PlayerMovementConfig
        {
            [field: SerializeField, Tooltip("Walk speen in m/s.")]
            public float MoveSpeed { get; private set; } = 4f;

            [field: SerializeField, Range(-50f, 0f), Tooltip("Gravity acceleration (negative).")]
            public float Gravity { get; private set; } = -20f;
            
            [field: SerializeField, Range(-10f, 0f), Tooltip("Downward velocity kept while grounded so isGrounded stays stable on slopes/steps.")]
            public float GroundedStickVelocity { get; private set; } = -2f;
        }

        [Serializable]
        public class PlayerLookConfig
        {
            [field: SerializeField, Range(0.01f, 1f), Tooltip("Degrees of rotation per pixel of mouse movement.")]
            public float Sensitivity { get; private set; } = 0.1f;

            [field: SerializeField]
            public bool InvertY { get; private set; }

            [field: SerializeField, Range(-89f, 0f)]
            public float MinPitch { get; private set; } = -85f;

            [field: SerializeField, Range(0f, 89f)]
            public float MaxPitch { get; private set; } = 85f;
        }
        
        [Serializable]
        public class InteractionConfig
        {
            [field: SerializeField, Range(0.5f, 10f), Tooltip("How far the player can reach, in metres.")]
            public float Range { get; private set; } = 2.5f;

            [field: SerializeField, Tooltip("Layers the interaction ray can hit (interactables AND walls that should block it).")]
            public LayerMask InteractionMask { get; private set; } = Physics.DefaultRaycastLayers;
        }
        
        [Serializable]
        public class HighlightConfig
        {
            [field: SerializeField, Tooltip("Shader colour property. URP Lit/Unlit: _BaseColor. Built-in: _Color.")]
            public string ColorPropertyName { get; private set; } = "_BaseColor";

            [field: SerializeField]
            public Color PickupColor { get; private set; } = new(1f, 0.85f, 0.2f);

            [field: SerializeField]
            public Color PlacementSpotColor { get; private set; } = new(0.2f, 0.9f, 1f);

            [field: SerializeField, Range(0f, 1f), Tooltip("How strongly the highlight colour replaces the original.")]
            public float Strength { get; private set; } = 0.6f;

            [field: SerializeField, Range(0f, 20f), Tooltip("Pulse speed. 0 = static tint.")]
            public float PulseSpeed { get; private set; } = 4f;

            public Color GetColor(HighlightStyle style) =>
                style == HighlightStyle.PlacementSpot ? PlacementSpotColor : PickupColor;
        }
    }
}
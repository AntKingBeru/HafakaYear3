using System;
using Hafaka.Core;
using UnityEngine;

namespace Hafaka.Interactions.Highlighting
{
    public class TintHighlight : HighlightBase
    {
        [SerializeField] private GameConfig.HighlightStyle style = GameConfig.HighlightStyle.Pickup;
        [Tooltip("Renderers to tint. Empty = all renderers on this object and its children (collected once).")]
        [SerializeField] private Renderer[] targetRenderers;

        private MaterialPropertyBlock _block;
        private Color?[][] _originalColors; // NOTE: [renderer][materialIndex]; null = material lacks the color property.
        private int _colorPropertyId;
        private bool _isInitialized;
        private bool _isHighlighted;

        private void Awake()
        {
            EnsureInitialized();
            enabled = _isHighlighted; // NOTE: Update (the pulse) only runs while highlighted.
        }

        public override void SetHighlighted(bool isHighlighted)
        {
            EnsureInitialized();
            if (_isHighlighted == isHighlighted)
                return;

            _isHighlighted = isHighlighted;
            enabled = isHighlighted;

            if (isHighlighted)
                ApplyTint(GetTintAmount());
            else
                ClearTint();
        }

        private void Update() => ApplyTint(GetTintAmount());

        private float GetTintAmount()
        {
            GameConfig.HighlightConfig config = GameConfig.Highlight;
            if (config.PulseSpeed <= 0f)
                return config.Strength;

            float wave = (Mathf.Sin(Time.unscaledTime * config.PulseSpeed) + 1f) * 0.5f; // 0..1
            return config.Strength * Mathf.Lerp(0.5f, 1f, wave);
        }

        private void ApplyTint(float amount)
        {
            Color target = GameConfig.Highlight.GetColor(style);
            for (var r = 0; r < targetRenderers.Length; r++)
            {
                Renderer rend = targetRenderers[r];
                if (!rend)
                    continue;

                Color?[] originals = _originalColors[r];
                for (var m = 0; m < originals.Length; m++)
                {
                    if (!originals[m].HasValue)
                        continue;
                    _block.Clear();
                    _block.SetColor(_colorPropertyId, Color.Lerp(originals[m].Value, target, amount));
                    rend.SetPropertyBlock(_block, m);
                }
            }
        }

        // NOTE: Passing null removes the override, restoring the material (and SRP Batcher compatibility).
        private void ClearTint()
        {
            for (var r = 0; r < targetRenderers.Length; r++)
            {
                Renderer rend = targetRenderers[r];
                if (!rend)
                    continue;
                for (var m = 0; m < _originalColors[r].Length; m++)
                    if (_originalColors[r][m].HasValue)
                        rend.SetPropertyBlock(null, m);
            }
        }

        // NOTE: Lazy so SetHighlighted works even if called before Awake (e.g. on an inactive marker).
        private void EnsureInitialized()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;

            if (targetRenderers == null || targetRenderers.Length == 0)
                targetRenderers = GetComponentsInChildren<Renderer>(true);

            _block = new MaterialPropertyBlock();
            _colorPropertyId = Shader.PropertyToID(GameConfig.Highlight.ColorPropertyName);
            _originalColors = new Color?[targetRenderers.Length][];

            for (var r = 0; r < targetRenderers.Length; r++)
            {
                Renderer rend = targetRenderers[r];
                Material[] materials = rend ? rend.sharedMaterials : Array.Empty<Material>();
                _originalColors[r] = new Color?[materials.Length];

                for (var m = 0; m < materials.Length; m++)
                {
                    Material mat = materials[m];
                    _originalColors[r][m] = mat && mat.HasProperty(_colorPropertyId)
                        ? mat.GetColor(_colorPropertyId)
                        : null;
                }
            }
        }
    }
}
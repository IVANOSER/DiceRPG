using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniatureHitFlash : MonoBehaviour
{
    [Header("Renderers (optional). If empty, auto-finds in children.")]
    public Renderer[] renderers;

    [Header("Flash")]
    public Color defaultFlashColor = Color.white;
    [Range(0f, 2f)] public float tintStrength = 0.35f;   // how strong to tint base color
    public float flashDuration = 0.10f;

    // Common shader properties
    static readonly int BaseColor = Shader.PropertyToID("_BaseColor"); // URP Lit
    static readonly int ColorProp = Shader.PropertyToID("_Color");     // Standard
    static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    MaterialPropertyBlock _mpb;
    Coroutine _routine;

    struct RendererDefaults
    {
        public bool hasBaseColor;
        public Color baseColor;

        public bool hasColor;
        public Color color;

        public bool hasEmission;
        public Color emission;
    }

    readonly Dictionary<Renderer, RendererDefaults> _defaults = new();

    void Awake()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>(true);

        _mpb = new MaterialPropertyBlock();
        CacheDefaults();
        RestoreDefaults(); // ensure clean start
    }

    void OnDisable()
    {
        // Very important: if object disables mid-flash, restore!
        RestoreDefaults();
    }

    public void Play() => Play(defaultFlashColor);

    public void Play(Color flashColor)
    {
        if (!isActiveAndEnabled) return;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(FlashRoutine(flashColor));
    }

    IEnumerator FlashRoutine(Color flashColor)
    {
        float t = 0f;
        while (t < flashDuration)
        {
            float n = t / flashDuration;
            float k = 1f - Mathf.SmoothStep(0f, 1f, n); // quick in, smooth out

            ApplyFlash(flashColor, k);

            t += Time.deltaTime;
            yield return null;
        }

        RestoreDefaults();
        _routine = null;
    }

    void CacheDefaults()
    {
        _defaults.Clear();

        foreach (var r in renderers)
        {
            if (!r) continue;

            var mat = r.sharedMaterial;
            if (!mat) continue;

            var d = new RendererDefaults();

            if (mat.HasProperty(BaseColor))
            {
                d.hasBaseColor = true;
                d.baseColor = mat.GetColor(BaseColor);
            }

            if (mat.HasProperty(ColorProp))
            {
                d.hasColor = true;
                d.color = mat.GetColor(ColorProp);
            }

            if (mat.HasProperty(EmissionColor))
            {
                d.hasEmission = true;
                d.emission = mat.GetColor(EmissionColor);
            }

            _defaults[r] = d;
        }
    }

    void ApplyFlash(Color flashColor, float amount01)
    {
        foreach (var r in renderers)
        {
            if (!r) continue;
            if (!_defaults.TryGetValue(r, out var d)) continue;

            r.GetPropertyBlock(_mpb);
            _mpb.Clear();

            // Tint base color slightly toward flashColor (no permanent overwrite)
            if (d.hasBaseColor)
            {
                Color tinted = Color.Lerp(d.baseColor, flashColor, amount01 * tintStrength);
                _mpb.SetColor(BaseColor, tinted);
            }
            if (d.hasColor)
            {
                Color tinted = Color.Lerp(d.color, flashColor, amount01 * tintStrength);
                _mpb.SetColor(ColorProp, tinted);
            }

            // Soft emission pulse (optional)
            if (d.hasEmission)
            {
                Color e = Color.Lerp(d.emission, flashColor, amount01) * (amount01 * 1.25f);
                _mpb.SetColor(EmissionColor, e);
            }

            r.SetPropertyBlock(_mpb);
        }
    }

    void RestoreDefaults()
    {
        foreach (var r in renderers)
        {
            if (!r) continue;
            if (!_defaults.TryGetValue(r, out var d)) continue;

            r.GetPropertyBlock(_mpb);
            _mpb.Clear();

            if (d.hasBaseColor) _mpb.SetColor(BaseColor, d.baseColor);
            if (d.hasColor) _mpb.SetColor(ColorProp, d.color);
            if (d.hasEmission) _mpb.SetColor(EmissionColor, d.emission);

            r.SetPropertyBlock(_mpb);
        }
    }
}

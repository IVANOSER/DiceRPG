using UnityEngine;

public class DiceCoreVisualController : MonoBehaviour
{
    [Header("Core Renderer")]
    [SerializeField] private Renderer coreRenderer;

    [Header("Shader Property")]
    [SerializeField] private string colorProperty = "_EnergyColor";

    [Header("Fallback")]
    [SerializeField] private Color emptyColor = Color.black;

    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        mpb = new MaterialPropertyBlock();
    }

    public void ApplyFromRuntime()
    {
        var rt = DiceLoadoutRuntime.Instance;
        if (rt == null || coreRenderer == null) return;

        Color c = rt.Ultimate != null
            ? rt.Ultimate.coreColor
            : emptyColor;

        coreRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(colorProperty, c);
        coreRenderer.SetPropertyBlock(mpb);
    }
}

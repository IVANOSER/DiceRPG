using UnityEngine;

public class EnemyHighlight : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color highlightColor = Color.red;
    [SerializeField] private float emissionIntensity = 2f;

    private MaterialPropertyBlock block;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        block = new MaterialPropertyBlock();

        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();
    }

    public void SetHighlighted(bool highlighted)
    {
        foreach (var r in renderers)
        {
            r.GetPropertyBlock(block);

            if (highlighted)
            {
                block.SetColor(EmissionColor, highlightColor * emissionIntensity);
            }
            else
            {
                block.SetColor(EmissionColor, Color.black);
            }

            r.SetPropertyBlock(block);
        }
    }
}

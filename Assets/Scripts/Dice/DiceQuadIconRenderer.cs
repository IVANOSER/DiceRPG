using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DiceQuadIconRenderer : MonoBehaviour
{
    [Header("Target renderer (your Quad)")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Shader texture properties (auto-tries these)")]
    [SerializeField] private string[] textureProps = { "_BaseMap", "_MainTex" };
    [SerializeField] private string[] stProps = { "_BaseMap_ST", "_MainTex_ST" };

    [Header("Optional")]
    [SerializeField] private Sprite emptySprite;              // якщо хочеш очищення через спрайт
    [SerializeField] private bool hideRendererWhenNull = true; // якщо true — при null просто ховаємо renderer

    private MaterialPropertyBlock _mpb;

    private void Awake()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();
    }

    public void SetSkillIcon(SkillSO skill)
    {
        Sprite sprite = (skill != null) ? skill.icon : null;

        // якщо треба очищення — використовуємо emptySprite
        if (sprite == null && emptySprite != null)
            sprite = emptySprite;

        if (!targetRenderer) return;

        // Якщо все одно null (немає emptySprite)
        if (sprite == null)
        {
            if (hideRendererWhenNull)
                targetRenderer.enabled = false;

            // НЕ сетимо null texture, щоб не падало
            return;
        }

        // Якщо є спрайт — показуємо renderer
        targetRenderer.enabled = true;

        Texture tex = sprite.texture;
        Vector4 st = ComputeST(sprite);

        targetRenderer.GetPropertyBlock(_mpb);
        ApplyTextureAndST(tex, st);
        targetRenderer.SetPropertyBlock(_mpb);
    }

    private void ApplyTextureAndST(Texture tex, Vector4 st)
    {
        // tex (ВАЖЛИВО: tex тут завжди НЕ null)
        foreach (var p in textureProps)
        {
            if (targetRenderer.sharedMaterial != null && targetRenderer.sharedMaterial.HasProperty(p))
            {
                _mpb.SetTexture(p, tex);
                break;
            }
        }

        // ST (tiling/offset packed as Vector4: x=scaleX, y=scaleY, z=offsetX, w=offsetY)
        foreach (var p in stProps)
        {
            if (targetRenderer.sharedMaterial != null && targetRenderer.sharedMaterial.HasProperty(p))
            {
                _mpb.SetVector(p, st);
                break;
            }
        }
    }

    private static Vector4 ComputeST(Sprite sprite)
    {
        Rect r = sprite.rect;
        Texture2D tex = sprite.texture;

        float scaleX = r.width / tex.width;
        float scaleY = r.height / tex.height;

        float offsetX = r.x / tex.width;
        float offsetY = r.y / tex.height;

        return new Vector4(scaleX, scaleY, offsetX, offsetY);
    }
}

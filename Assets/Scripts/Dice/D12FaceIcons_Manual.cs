using UnityEngine;

[ExecuteAlways]
public class D12FaceIcons_Manual : MonoBehaviour
{
    [Header("Assign 12 quad MeshRenderers in order: Face1..Face12")]
    [SerializeField] private MeshRenderer[] quads = new MeshRenderer[12];

    [SerializeField] private Material iconMaterialTemplate;
    [SerializeField] private bool hideWhenNull = true;

    private Material[] mats;

    private void OnEnable()
    {
        PrepareMaterials();
    }

    private void OnValidate()
    {
        PrepareMaterials();
    }

    [ContextMenu("Prepare Materials")]
    public void PrepareMaterials()
    {
        if (quads == null || quads.Length != 12)
            quads = new MeshRenderer[12];

        mats = new Material[12];

        for (int i = 0; i < 12; i++)
        {
            var r = quads[i];
            if (r == null) continue;

            // прибрати collider, якщо випадково є
            var col = r.GetComponent<Collider>();
            if (col != null)
            {
                if (Application.isPlaying) Destroy(col);
                else DestroyImmediate(col);
            }

            // унікальний material instance на грань
            mats[i] = iconMaterialTemplate != null
                ? new Material(iconMaterialTemplate)
                : new Material(Shader.Find("Unlit/Texture"));

            r.material = mats[i];
        }
    }

    public void Apply(SkillSO[] faces)
    {
        if (faces == null || faces.Length < 12)
        {
            Debug.LogError("[D12 Manual Icons] Apply expects SkillSO[12]");
            return;
        }

        if (mats == null || mats.Length != 12)
            PrepareMaterials();

        for (int i = 0; i < 12; i++)
        {
            var r = quads[i];
            if (r == null) continue;

            var s = faces[i];

            if (s == null || s.icon == null)
            {
                if (hideWhenNull) r.enabled = false;
                else r.material.mainTexture = null;
                continue;
            }

            r.enabled = true;
            r.material.mainTexture = s.icon.texture; // MVP: окремі PNG — ідеально
        }
    }
}

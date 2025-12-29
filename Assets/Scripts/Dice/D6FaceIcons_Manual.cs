using UnityEngine;

[ExecuteAlways]
public class D6FaceIcons_Manual : MonoBehaviour
{
    [Header("Assign 6 quad MeshRenderers in order: Face1..Face6")]
    [SerializeField] private MeshRenderer[] quads = new MeshRenderer[6];

    [SerializeField] private Material iconMaterialTemplate;
    [SerializeField] private bool hideWhenNullOrEmpty = true;

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
        if (quads == null || quads.Length != 6)
            quads = new MeshRenderer[6];

        mats = new Material[6];

        for (int i = 0; i < 6; i++)
        {
            var r = quads[i];
            if (r == null) continue;

            var col = r.GetComponent<Collider>();
            if (col != null) DestroyImmediate(col);

            mats[i] = iconMaterialTemplate != null
                ? new Material(iconMaterialTemplate)
                : new Material(Shader.Find("Unlit/Texture"));

            r.material = mats[i];
        }
    }

    public void Apply(ModifierSO[] faces)
    {
        if (faces == null || faces.Length < 6) return;

        if (mats == null || mats.Length != 6)
            PrepareMaterials();

        for (int i = 0; i < 6; i++)
        {
            var r = quads[i];
            if (r == null) continue;

            var m = faces[i];
            bool empty = (m == null) || m.isEmpty || m.icon == null;

            if (empty)
            {
                if (hideWhenNullOrEmpty) r.enabled = false;
                else r.material.mainTexture = null;
                continue;
            }

            r.enabled = true;
            r.material.mainTexture = m.icon.texture;
        }
    }
}

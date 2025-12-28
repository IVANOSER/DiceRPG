using UnityEngine;

public class D6FaceIcons_Positions : MonoBehaviour
{
    [Header("Root with Position1..Position6")]
    [SerializeField] private Transform positionsRoot;

    [Header("Material template (Unlit/Transparent)")]
    [SerializeField] private Material iconMaterialTemplate;

    [Header("Names")]
    [SerializeField] private string prefix = "Position"; // Position1..6
    [SerializeField] private bool oneBased = true;

    private MeshRenderer[] renderers = new MeshRenderer[6];
    private bool cached;

    private void Awake()
    {
        Cache();
    }

    [ContextMenu("Cache Now")]
    public void Cache()
    {
        if (positionsRoot == null)
        {
            var faces = transform.Find("Faces");
            positionsRoot = faces != null ? faces : transform;
        }

        for (int i = 0; i < 6; i++) renderers[i] = null;

        for (int n = 1; n <= 6; n++)
        {
            Transform t = positionsRoot.Find($"{prefix}{n}");
            if (t == null) t = positionsRoot.Find($"{prefix}_{n}");
            if (t == null)
            {
                Debug.LogError($"[D6 Icons] Missing {prefix}{n} under {positionsRoot.name}");
                continue;
            }

            int idx = oneBased ? (n - 1) : n;
            if (idx < 0 || idx > 5) continue;

            var r = t.GetComponentInChildren<MeshRenderer>(true);
            if (r == null)
            {
                Debug.LogError($"[D6 Icons] No MeshRenderer under {t.name}");
                continue;
            }

            if (iconMaterialTemplate != null)
                r.material = new Material(iconMaterialTemplate);
            else
                r.material = new Material(r.material);

            var col = r.GetComponent<Collider>();
            if (col != null) Destroy(col);

            renderers[idx] = r;
        }

        cached = true;
        Debug.Log($"[D6 Icons] Cached {CountCached()}/6 from {positionsRoot.name}");
    }

    private int CountCached()
    {
        int c = 0;
        for (int i = 0; i < 6; i++) if (renderers[i] != null) c++;
        return c;
    }

    public void Apply(ModifierSO[] faces)
    {
        if (!cached) Cache();
        if (faces == null || faces.Length != 6)
        {
            Debug.LogError("[D6 Icons] Apply expects ModifierSO[6]");
            return;
        }

        for (int i = 0; i < 6; i++)
        {
            var r = renderers[i];
            if (r == null) continue;

            var m = faces[i];
            if (m == null || m.isEmpty || m.icon == null)
            {
                r.enabled = false;
                continue;
            }

            r.enabled = true;
            r.material.mainTexture = m.icon.texture;
        }
    }
}

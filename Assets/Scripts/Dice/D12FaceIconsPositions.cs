using UnityEngine;

public class D12FaceIcons_Positions : MonoBehaviour
{
    [Header("Root with Position1..Position12 (optional)")]
    [SerializeField] private Transform positionsRoot; // якщо null — root кубика

    [Header("Material template (Unlit/Transparent)")]
    [SerializeField] private Material iconMaterialTemplate;

    [Header("Naming")]
    [SerializeField] private string prefix = "Position"; // Position11 / Position_11 / Position 11
    [SerializeField] private bool oneBased = true;

    private MeshRenderer[] renderers = new MeshRenderer[12];
    private bool cached;

    private void Awake()
    {
        Cache();
    }

    [ContextMenu("Cache Now")]
    public void Cache()
{
    if (positionsRoot == null)
        positionsRoot = transform;

    // debug: що реально є під root
    string children = "";
    for (int i = 0; i < positionsRoot.childCount; i++)
        children += positionsRoot.GetChild(i).name + ", ";
    Debug.Log($"[D12 Icons] Root={positionsRoot.name} children: {children}");

    for (int i = 0; i < 12; i++) renderers[i] = null;

    for (int n = 1; n <= 12; n++)
    {
        Transform t = positionsRoot.Find($"Position{n}");
        if (t == null)
        {
            Debug.LogError($"[D12 Icons] Missing Position{n} under {positionsRoot.name}");
            continue;
        }

        int idx = n - 1;

        var r = t.GetComponentInChildren<MeshRenderer>(true);
        if (r == null)
        {
            Debug.LogError($"[D12 Icons] No MeshRenderer under {t.name}");
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
    Debug.Log($"[D12 Icons] Cached {CountCached()}/12 (root={positionsRoot.name})");
}


    private Transform FindPositionTransformRecursive(Transform root, int n)
    {
        // приймаємо кілька варіантів імен
        string a = $"{prefix}{n}";
        string b = $"{prefix}_{n}";
        string c = $"{prefix} {n}";

        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == a || t.name == b || t.name == c)
                return t;
        }
        return null;
    }

    private int CountCached()
    {
        int c = 0;
        for (int i = 0; i < 12; i++) if (renderers[i] != null) c++;
        return c;
    }

    public void Apply(SkillSO[] faces)
    {
        if (!cached) Cache();
        if (faces == null || faces.Length != 12)
        {
            Debug.LogError("[D12 Icons] Apply expects SkillSO[12]");
            return;
        }

        for (int i = 0; i < 12; i++)
        {
            var r = renderers[i];
            if (r == null) continue;

            var s = faces[i];
            if (s == null || s.icon == null)
            {
                r.enabled = false;
                continue;
            }

            r.enabled = true;
            r.material.mainTexture = s.icon.texture;
        }
    }
}

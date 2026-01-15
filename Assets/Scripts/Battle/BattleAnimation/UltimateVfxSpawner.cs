using UnityEngine;

public class UltimateVfxSpawner : MonoBehaviour
{
    public static UltimateVfxSpawner Instance;

    [Header("Prefabs")]
    public GameObject meteorPrefab;
    public GameObject lightningPrefab;

    [Header("Scene anchors")]
    public Transform vfxRoot; // optional: щоб все було під одним батьком

    [Header("Timing")]
    public float meteorLifeTime = 2.5f;
    public float lightningLifeTime = 1.2f;

    private void Awake()
    {
        Instance = this;
    }

    public void Play(UltimateSO ultimate, UltimateContext ctx, UltimateConfig cfg)
    {
        if (ultimate == null) return;

        // По presetKey ти вже розрізняєш типи ("single", "aoe", "self") :contentReference[oaicite:3]{index=3}
        // Але нам треба саме “яка ульта”: Метеор/Блискавка/Щит.
        // Найпростіше: завести enum у UltimateSO (нижче).
    }

    public void SpawnMeteor(Vector3 targetPos)
    {
        if (meteorPrefab == null) return;

        var go = Instantiate(meteorPrefab, vfxRoot ? vfxRoot : null);
        go.transform.position = targetPos;

        Destroy(go, meteorLifeTime);
    }

    public void SpawnLightning(Vector3 targetPos)
    {
        if (lightningPrefab == null) return;

        var go = Instantiate(lightningPrefab, vfxRoot ? vfxRoot : null);
        go.transform.position = targetPos;

        Destroy(go, lightningLifeTime);
    }
}

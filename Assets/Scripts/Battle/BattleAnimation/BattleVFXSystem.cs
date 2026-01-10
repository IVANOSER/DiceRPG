using UnityEngine;

public class BattleVFXSystem : MonoBehaviour
{
    public static BattleVFXSystem I { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject healPrefab;
    [SerializeField] private GameObject hitImpactPrefab;

    [Header("Spawn Offsets (tabletop)")]
    [SerializeField] private Vector3 healOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] private Vector3 hitOffset = new Vector3(0f, 0.35f, 0f);

    [Header("Optional: parent all spawned vfx here")]
    [SerializeField] private Transform vfxRoot;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }



    public void SpawnHeal(Transform target)
    {
        if (target == null || healPrefab == null) return;

        Vector3 pos = target.position + healOffset;
        Quaternion rot = target.rotation; 

        Spawn(healPrefab, pos, rot);
    }

    public void SpawnHitImpact(Transform target)
    {
        if (target == null || hitImpactPrefab == null) return;

        Spawn(
            hitImpactPrefab,
            target.position + hitOffset,
            target.rotation 
        );
    }


    private void Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (vfxRoot != null)
            Instantiate(prefab, pos, rot, vfxRoot);
        else
            Instantiate(prefab, pos, rot);
    }

}

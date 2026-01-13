using UnityEngine;

public class UltimatePersistent : MonoBehaviour
{
    public static UltimatePersistent Instance { get; private set; }

    public UltimateSO ultimate;
    public int maxCharge;
    public int chargePerAttack;
    public int currentCharge;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;
        var go = new GameObject("UltimatePersistent");
        go.AddComponent<UltimatePersistent>();
    }
}

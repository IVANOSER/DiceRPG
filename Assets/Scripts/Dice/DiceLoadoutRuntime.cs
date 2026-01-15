using System;
using UnityEngine;
using System.Linq;


public class DiceLoadoutRuntime : MonoBehaviour
{
    public static DiceLoadoutRuntime Instance { get; private set; }
    public static event Action OnChanged;
    public static event Action<UltimateSO, UltimateContext> OnUltimateUsed;


    [Header("Skill faces (12)")]
    [SerializeField] private SkillSO[] skillFaces = new SkillSO[12];
    public SkillSO[] SkillFaces => skillFaces;

    // ===================== ULTIMATE RUNTIME =====================
    [Header("Ultimate")]
    [SerializeField] private UltimateSO ultimate;
    [SerializeField] private int currentUltimateCharge;

    public UltimateSO Ultimate => ultimate;
    public int CurrentUltimateCharge => currentUltimateCharge;

    // ===================== AUTO CREATE =====================
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;

        var go = new GameObject("DiceLoadoutRuntime");
        go.AddComponent<DiceLoadoutRuntime>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (skillFaces == null || skillFaces.Length != 12)
            skillFaces = new SkillSO[12];
    }

    // ===================== SKILLS =====================
    public void RebuildSkillFacesFromEquipped(SkillSO[] equipped6)
    {
        int idx = 0;

        for (int i = 0; i < 6; i++)
        {
            SkillSO s = (equipped6 != null && i < equipped6.Length) ? equipped6[i] : null;

            if (idx < 12) skillFaces[idx++] = s;
            if (idx < 12) skillFaces[idx++] = s;
        }

        for (; idx < 12; idx++)
            skillFaces[idx] = null;

        OnChanged?.Invoke();
    }

    // ===================== ULTIMATE =====================
    public void SetUltimate(UltimateSO newUltimate)
    {
        ultimate = newUltimate;
        currentUltimateCharge = 0;
        OnChanged?.Invoke();
    }

    public void AddUltimateCharge(int amount, int maxCharge)
    {
        if (ultimate == null) return;

        int before = currentUltimateCharge;
        currentUltimateCharge = Mathf.Clamp(currentUltimateCharge + Mathf.Max(0, amount), 0, maxCharge);

        if (currentUltimateCharge != before)
            OnChanged?.Invoke();
    }

    public bool IsUltimateReady(int maxCharge)
    {
        return ultimate != null && currentUltimateCharge >= maxCharge;
    }

    public void ConsumeUltimate()
    {
        currentUltimateCharge = 0;
        OnChanged?.Invoke();
    }
    public bool TryUseUltimate()
{
    if (ultimate == null)
        return false;

    var cfg = UltimateConfigLoader.Get();
    if (!IsUltimateReady(cfg.charge.maxCharge))
        return false;

    var bm = BattleManager.Instance;

    Enemy target = bm != null ? bm.selectedEnemy : null;
    Enemy[] allEnemies = bm != null ? bm.AliveEnemies.ToArray() : null;


    var player = UnityEngine.Object.FindFirstObjectByType<PlayerHealth>();

    if (player == null)
    {
        Debug.LogWarning("PlayerHealth not found for ultimate");
        return false;
    }

    var ctx = new UltimateContext
    {
        playerRoot = player.gameObject,
        target = target,
        allEnemies = allEnemies,
        turnManager = TurnManager.Instance
    };

    ultimate.Execute(ctx, cfg);
    OnUltimateUsed?.Invoke(ultimate, ctx);
    ConsumeUltimate();
    return true;
}

}

using System.Collections;
using System.Reflection;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public bool IsPlayerTurn { get; private set; } = true;
    public bool HasRolledThisTurn { get; private set; } = false;

    [Header("Player refs")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;

    public SkillSO PendingSkill { get; private set; }

    [Header("Rerolls (defaults if JSON missing)")]
    [SerializeField] private int maxRerollsPerBattle = 2;

    public int RerollsLeft { get; private set; }

    private int rerollGemCost = 15;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ApplyRerollConfigFromJson();
        RerollsLeft = maxRerollsPerBattle;
        BeginPlayerTurn();
    }

    private void ApplyRerollConfigFromJson()
    {
        var cfg = DiceRerollJsonConfig.Get();
        maxRerollsPerBattle = Mathf.Max(0, cfg.freeRerollsPerBattle);
        rerollGemCost = Mathf.Max(0, cfg.rerollGemCost);

        Debug.Log($"[TurnManager] Reroll config: free={maxRerollsPerBattle}, gemCost={rerollGemCost}");
    }

    // -------- Roll gating --------
    public bool CanRollDice()
    {
        return IsPlayerTurn && !HasRolledThisTurn;
    }

    public void MarkRolled()
    {
        HasRolledThisTurn = true;
        RefreshRollButton();
    }

    private void ResetRollForNewTurn()
    {
        HasRolledThisTurn = false;
        RefreshRollButton();
    }

    // -------- Pending skill --------
    public void SetPendingSkill(SkillSO skill)
    {
        if (!IsPlayerTurn) return;
        PendingSkill = skill;
        RefreshUI();
        RefreshRerollButton();
    }

    // -------- Action --------
    public void OnActionPressed()
    {
        if (!IsPlayerTurn) return;
        if (PendingSkill == null) return;

        if (PendingSkill.type == SkillType.Attack)
        {
            if (BattleManager.Instance == null || BattleManager.Instance.selectedEnemy == null)
            {
                RefreshUI();
                return;
            }

            int dmgBase = (playerStats != null) ? playerStats.Damage : 1;
            int dmg = dmgBase + PendingSkill.baseValue;
            BattleManager.Instance.AttackSelected(dmg);
        }
        else if (PendingSkill.type == SkillType.Heal)
        {
            int heal = Mathf.Max(0, PendingSkill.baseValue);
            playerHealth.Heal(heal);
        }

        PendingSkill = null;
        RefreshUI();
        RefreshRerollButton();
        EndPlayerTurn();
    }

    public bool CanPressAction()
    {
        if (!IsPlayerTurn) return false;
        if (PendingSkill == null) return false;

        if (PendingSkill.type == SkillType.Attack)
            return BattleManager.Instance != null && BattleManager.Instance.selectedEnemy != null;

        return true;
    }

    public void RefreshActionState()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.SetActionInteractable(CanPressAction(), PendingSkill);
    }

    // -------- Turn flow --------
    public void BeginPlayerTurn()
    {
        IsPlayerTurn = true;
        HasRolledThisTurn = false;
        PendingSkill = null;
        RefreshUI();
        RefreshRerollButton();
    }

    private void EndPlayerTurn()
    {
        if (!IsPlayerTurn) return;

        IsPlayerTurn = false;

        RefreshRollButton();
        RefreshUI();

        StartCoroutine(EnemiesTurn());
    }

    private IEnumerator EnemiesTurn()
    {
        foreach (var enemy in BattleManager.Instance.AliveEnemies)
        {
            if (enemy == null) continue;

            playerHealth.TakeDamage(enemy.AttackDamage);
            yield return new WaitForSeconds(0.4f);

            if (playerHealth.CurrentHp <= 0)
                yield break;
        }

        BeginPlayerTurn();
    }

    // -------- UI refresh --------
    private void RefreshUI()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.Refresh(0, IsPlayerTurn);

        RefreshActionState();
        RefreshRollButton();
    }

    private void RefreshRollButton()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.SetRollInteractable(CanRollDice());
    }

    public void ResetAttacksForNewWave()
    {
        BeginPlayerTurn();
    }

    // ===================== REROLL LOGIC (FREE + PAID) =====================

    public bool CanReroll()
    {
        // рерол тільки на ходу гравця, тільки якщо вже є PendingSkill
        if (!IsPlayerTurn || PendingSkill == null)
            return false;

        // free rerolls
        if (RerollsLeft > 0)
            return true;

        // paid reroll (if gems enough)
        if (rerollGemCost <= 0)
            return true;

        return CurrencyWalletBridge.HasGems(rerollGemCost);
    }

    /// <summary>
    /// Try to use free reroll; if no free left, try to buy reroll for gems.
    /// Returns true if reroll is allowed and paid/free was consumed.
    /// </summary>
    public bool TryConsumeRerollOrBuy()
    {
        if (!IsPlayerTurn || PendingSkill == null)
            return false;

        if (RerollsLeft > 0)
        {
            ConsumeReroll();
            return true;
        }

        int cost = Mathf.Max(0, rerollGemCost);
        if (cost <= 0) return true;

        if (!CurrencyWalletBridge.HasGems(cost))
            return false;

        CurrencyWalletBridge.SpendGems(cost);
        RefreshRerollButton();
        return true;
    }

    public void ConsumeReroll()
    {
        RerollsLeft = Mathf.Max(0, RerollsLeft - 1);
        RefreshRerollButton();
    }

    private void RefreshRerollButton()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.SetRerollVisible(IsPlayerTurn && PendingSkill != null);

        if (BattleUI.Instance != null)
            BattleUI.Instance.SetRerollInteractable(CanReroll(), RerollsLeft);
    }

    // ===================== Currency Wallet Bridge (NO compile dependency) =====================
    // This tries to find CurrencyWallet / CurrancyWallet and call common methods via reflection.
    private static class CurrencyWalletBridge
    {
        private static Object _cachedInstance;
        private static System.Type _cachedType;

        private static bool Ensure()
        {
            if (_cachedInstance != null && _cachedType != null) return true;

            // Find by type name (common spellings)
            var all = Object.FindObjectsOfType<MonoBehaviour>(true);
            foreach (var mb in all)
            {
                if (mb == null) continue;

                var t = mb.GetType();
                if (t.Name == "CurrencyWallet" || t.Name == "CurrancyWallet")
                {
                    _cachedInstance = mb;
                    _cachedType = t;
                    return true;
                }
            }

            // also try static Instance property on loaded types (optional)
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Type t = asm.GetType("CurrencyWallet") ?? asm.GetType("CurrancyWallet");
                if (t == null) continue;

                var instProp = t.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instProp != null)
                {
                    var inst = instProp.GetValue(null, null) as Object;
                    if (inst != null)
                    {
                        _cachedInstance = inst;
                        _cachedType = t;
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool HasGems(int amount)
        {
            if (amount <= 0) return true;
            if (!Ensure()) return false;

            // Try methods in order:
            // HasGems(int)
            if (TryInvokeBool("HasGems", amount, out bool b1)) return b1;
            // CanSpendGems(int)
            if (TryInvokeBool("CanSpendGems", amount, out bool b2)) return b2;
            // TrySpendGems(int) as check (NOT spending) - skip to avoid side effects

            // Try property Gems / CurrentGems / gems
            if (TryGetIntProp(new[] { "Gems", "CurrentGems", "gems" }, out int gems))
                return gems >= amount;

            Debug.LogWarning("[CurrencyWalletBridge] Can't find HasGems/CanSpendGems/Gems on wallet.");
            return false;
        }

        public static void SpendGems(int amount)
        {
            if (amount <= 0) return;
            if (!Ensure()) return;

            // TrySpendGems(int) returning bool
            if (TryInvokeBool("TrySpendGems", amount, out bool ok))
            {
                if (!ok) Debug.LogWarning("[CurrencyWalletBridge] TrySpendGems returned false.");
                return;
            }

            // SpendGems(int)
            if (TryInvokeVoid("SpendGems", amount)) return;

            // RemoveGems(int)
            if (TryInvokeVoid("RemoveGems", amount)) return;

            Debug.LogWarning("[CurrencyWalletBridge] Can't find Spend method (TrySpendGems/SpendGems/RemoveGems).");
        }

        private static bool TryInvokeBool(string method, int arg, out bool result)
        {
            result = false;
            var m = _cachedType.GetMethod(method, BindingFlags.Public | BindingFlags.Instance);
            if (m == null) return false;

            var p = m.GetParameters();
            if (p.Length != 1 || p[0].ParameterType != typeof(int)) return false;

            var r = m.Invoke(_cachedInstance, new object[] { arg });
            if (r is bool bb)
            {
                result = bb;
                return true;
            }
            return false;
        }

        private static bool TryInvokeVoid(string method, int arg)
        {
            var m = _cachedType.GetMethod(method, BindingFlags.Public | BindingFlags.Instance);
            if (m == null) return false;

            var p = m.GetParameters();
            if (p.Length != 1 || p[0].ParameterType != typeof(int)) return false;

            m.Invoke(_cachedInstance, new object[] { arg });
            return true;
        }

        private static bool TryGetIntProp(string[] names, out int value)
        {
            value = 0;
            foreach (var n in names)
            {
                var prop = _cachedType.GetProperty(n, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null && prop.PropertyType == typeof(int))
                {
                    value = (int)prop.GetValue(_cachedInstance, null);
                    return true;
                }

                var field = _cachedType.GetField(n, BindingFlags.Public | BindingFlags.Instance);
                if (field != null && field.FieldType == typeof(int))
                {
                    value = (int)field.GetValue(_cachedInstance);
                    return true;
                }
            }
            return false;
        }

    }
    public bool HasEnoughGemsForReroll()
        {
            if (rerollGemCost <= 0)
            return true;

            return CurrencyWalletBridge.HasGems(rerollGemCost);
        }

        public int GetRerollGemCost()
        {
            return rerollGemCost;
        }
}

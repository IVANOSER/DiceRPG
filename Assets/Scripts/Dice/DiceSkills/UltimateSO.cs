using UnityEngine;

// ✅ ці енумки потрібні твоїм UltimateSingleSO / UltimateAoESO / UltimateSelfSO
public enum UltimateStatusKind { None, Burn, Stun }
public enum UltimateSelfStatusKind { None, Shield }

public abstract class UltimateSO : ScriptableObject
{
    // =========================
    // SLOT (як у EquipItemSO)
    // =========================
    [Header("Slot")]
    [Tooltip("Ultimate slot. For now always DiceCore.")]
    public EquipmentSlot slot = EquipmentSlot.DiceCore;

    // =========================
    // UI
    // =========================
    [Header("UI")]
    public string displayName;
    public Sprite icon;

    // =========================
    // DICE CORE VISUAL
    // =========================
    [Header("Dice Core Visual")]
    [Tooltip("Color applied to Dice Core visual")]
    public Color coreColor = Color.white;

    // =========================
    // RUNTIME
    // =========================
    public abstract string PresetKey { get; }
    public abstract void Execute(UltimateContext ctx, UltimateConfig cfg);

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 🔒 гарантуємо, що ульта завжди для ядра (щоб не ламали руками)
        slot = EquipmentSlot.DiceCore;
    }
#endif
}

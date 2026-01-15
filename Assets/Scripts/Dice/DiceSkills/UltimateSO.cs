using UnityEngine;

// ✅ ці енумки потрібні твоїм UltimateSingleSO / UltimateAoESO / UltimateSelfSO
public enum UltimateStatusKind { None, Burn, Stun }
public enum UltimateSelfStatusKind { None, Shield }

public enum UltimateVfxId
{
    None,
    Meteor,
    Lightning,
    Shield
}


public abstract class UltimateSO : ScriptableObject
{
    public UltimateVfxId vfxId = UltimateVfxId.None;

    [Header("Slot")]
    [Tooltip("Ultimate slot. For now always DiceCore.")]
    public EquipmentSlot slot = EquipmentSlot.DiceCore;

    [Header("UI")]
    public string displayName;
    public Sprite icon;

    [Header("Dice Core Visual")]
    [Tooltip("Color applied to Dice Core visual")]
    public Color coreColor = Color.white;

    public abstract string PresetKey { get; }
    public abstract void Execute(UltimateContext ctx, UltimateConfig cfg);

#if UNITY_EDITOR
    private void OnValidate()
    {
        
        slot = EquipmentSlot.DiceCore;
    }

#endif
}






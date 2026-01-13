using UnityEngine;

public enum UltimateStatusKind { None, Burn, Stun }
public enum UltimateSelfStatusKind { None, Shield }

public abstract class UltimateSO : ScriptableObject
{
    [Header("UI")]
    public string displayName;
    public Sprite icon;

    // ключ, який каже які числа брати з json
    public abstract string PresetKey { get; }

    public abstract void Execute(UltimateContext ctx, UltimateConfig cfg);
}

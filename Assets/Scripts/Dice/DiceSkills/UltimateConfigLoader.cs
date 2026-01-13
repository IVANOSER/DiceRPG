using System.IO;
using UnityEngine;

public static class UltimateConfigLoader
{
    private static UltimateConfig _cached;

    public static UltimateConfig Get()
    {
        if (_cached != null) return _cached;

        string path = Path.Combine(Application.streamingAssetsPath, "ultimates.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.LogError("StreamingAssets on Android needs UnityWebRequest loader. If you build Android now — скажи.");
        _cached = new UltimateConfig();
        return _cached;
#else
        if (!File.Exists(path))
        {
            Debug.LogError($"ultimates.json not found at: {path}");
            _cached = new UltimateConfig();
            return _cached;
        }

        string json = File.ReadAllText(path);
        _cached = JsonUtility.FromJson<UltimateConfig>(json) ?? new UltimateConfig();
        return _cached;
#endif
    }

    public static void ClearCache() => _cached = null;
}

[System.Serializable]
public class UltimateConfig
{
    public ChargeConfig charge = new ChargeConfig();
    public AoEConfig aoe = new AoEConfig();
    public SingleConfig single = new SingleConfig();
    public SelfConfig self = new SelfConfig();
}

[System.Serializable] public class ChargeConfig { public int maxCharge = 100; public int chargePerAttack = 25; }

[System.Serializable] public class AoEConfig
{
    public int damage = 8;
    public int burnTurns = 2;
    public int burnDamagePerTurn = 3;
    public int stunTurns = 1;
}

[System.Serializable] public class SingleConfig
{
    public int damage = 18;
    public int burnTurns = 2;
    public int burnDamagePerTurn = 3;
    public int stunTurns = 1;
}

[System.Serializable] public class SelfConfig
{
    public int healAmount = 0;
    public bool healFull = false;
    public int shieldAbsorbs = 3;
}

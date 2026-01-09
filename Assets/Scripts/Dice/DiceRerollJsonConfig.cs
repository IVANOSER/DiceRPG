using System;
using UnityEngine;

[Serializable]
public class DiceRerollJsonConfigData
{
    public int freeRerollsPerBattle = 2;
    public int rerollGemCost = 15;
}

public static class DiceRerollJsonConfig
{
    private const string ResourcePath = "Data/Configs/dice_reroll"; // Resources/Configs/DiceRerollConfig.json
    private static DiceRerollJsonConfigData _cached;

    public static DiceRerollJsonConfigData Get()
    {
        if (_cached != null) return _cached;

        TextAsset json = Resources.Load<TextAsset>(ResourcePath);
        if (json == null)
        {
            Debug.LogWarning($"[DiceRerollConfig] Missing JSON at Resources/{ResourcePath}.json. Using defaults.");
            _cached = new DiceRerollJsonConfigData();
            return _cached;
        }

        try
        {
            _cached = JsonUtility.FromJson<DiceRerollJsonConfigData>(json.text);
            if (_cached == null) _cached = new DiceRerollJsonConfigData();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[DiceRerollConfig] Parse error. Using defaults. {e.Message}");
            _cached = new DiceRerollJsonConfigData();
        }

        // clamp safety
        _cached.freeRerollsPerBattle = Mathf.Max(0, _cached.freeRerollsPerBattle);
        _cached.rerollGemCost = Mathf.Max(0, _cached.rerollGemCost);

        return _cached;
    }

    public static void ResetCache() => _cached = null;
}

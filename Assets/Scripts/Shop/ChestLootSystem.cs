using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QualityWeightEntry
{
    public string quality; // "Good" / "Bttr" / "Rare" / "Leg"
    public int weight;
}

[Serializable]
public class QualityWeightsJson
{
    public List<QualityWeightEntry> weights = new();
}

[Serializable]
public class ChestConfigJson
{
    public string chestId;
    public int priceOpen1;
    public int priceOpen10;
}

public static class ChestLootSystem
{
    public static bool TryLoad(string chestConfigResourcesPath, string qualityWeightsResourcesPath,
        out ChestConfigJson chest, out QualityWeightsJson weights)
    {
        chest = null;
        weights = null;

        var chestTa = Resources.Load<TextAsset>(chestConfigResourcesPath);
        var weightsTa = Resources.Load<TextAsset>(qualityWeightsResourcesPath);

        if (chestTa == null)
        {
            Debug.LogError($"Chest config not found in Resources: {chestConfigResourcesPath}");
            return false;
        }
        if (weightsTa == null)
        {
            Debug.LogError($"Quality weights not found in Resources: {qualityWeightsResourcesPath}");
            return false;
        }

        chest = JsonUtility.FromJson<ChestConfigJson>(chestTa.text);
        weights = JsonUtility.FromJson<QualityWeightsJson>(weightsTa.text);

        return chest != null && weights != null;
    }

    public static List<EquipItemSO> RollItemsByQuality(
        QualityWeightsJson weights,
        string itemsResourcesPath, // "Data/Item"
        int count)
    {
        // 1) Завантажуємо всі айтеми з Resources/Data/Item/** (підпапки 001/002 ок)
        var all = Resources.LoadAll<EquipItemSO>(itemsResourcesPath);
        if (all == null || all.Length == 0)
        {
            Debug.LogError($"No EquipItemSO found at Resources/{itemsResourcesPath}");
            return new List<EquipItemSO>();
        }

        // 2) Групуємо по quality
        var byQ = new Dictionary<string, List<EquipItemSO>>(StringComparer.OrdinalIgnoreCase);
        foreach (var it in all)
        {
            if (it == null) continue;
            var q = it.quality.ToString(); // enum -> "Good" "Bttr"...
            if (!byQ.TryGetValue(q, out var list))
            {
                list = new List<EquipItemSO>();
                byQ[q] = list;
            }
            list.Add(it);
        }

        // 3) Підготовка ваг
        var weightList = new List<(string q, int w)>();
        int total = 0;

        foreach (var w in weights.weights)
        {
            if (string.IsNullOrEmpty(w.quality)) continue;
            int ww = Mathf.Max(0, w.weight);
            if (ww == 0) continue;

            weightList.Add((w.quality, ww));
            total += ww;
        }

        if (total <= 0)
        {
            Debug.LogError("Total quality weight is 0. Check quality_weights.json");
            return new List<EquipItemSO>();
        }

        // 4) Ролимо
        var result = new List<EquipItemSO>(count);

        for (int i = 0; i < count; i++)
        {
            string q = RollQuality(weightList, total);

            if (!byQ.TryGetValue(q, out var pool) || pool == null || pool.Count == 0)
            {
                // якщо для цієї якості нема айтемів — fallback: будь-який айтем
                result.Add(all[UnityEngine.Random.Range(0, all.Length)]);
                continue;
            }

            result.Add(pool[UnityEngine.Random.Range(0, pool.Count)]);
        }

        return result;
    }

    private static string RollQuality(List<(string q, int w)> list, int total)
    {
        int roll = UnityEngine.Random.Range(0, total);
        int acc = 0;
        for (int i = 0; i < list.Count; i++)
        {
            acc += list[i].w;
            if (roll < acc) return list[i].q;
        }
        return list[^1].q;
    }
}

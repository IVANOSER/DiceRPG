using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemUpgradeState
{
    public string itemId;
    public int level = 1;
    public int copies = 0;
}

public class ItemUpgradeManager : MonoBehaviour
{
    public static ItemUpgradeManager Instance { get; private set; }

    public event Action OnChanged;

    public int gold = 0;

    public int maxLevel = 10;
    public int baseCopiesNeed = 2;
    public int baseGoldNeed = 50;

    private readonly Dictionary<string, ItemUpgradeState> map = new();

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

    public ItemUpgradeState GetState(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return new ItemUpgradeState { itemId = "", level = 1, copies = 0 };

        if (!map.TryGetValue(itemId, out var s))
        {
            s = new ItemUpgradeState { itemId = itemId, level = 1, copies = 0 };
            map[itemId] = s;
        }

        return s;
    }

    public void AddCopies(EquipItemSO item, int amount = 1)
    {
        if (item == null) return;
        var s = GetState(item.Id);
        s.copies += Mathf.Max(0, amount);
        OnChanged?.Invoke();
    }

    public void AddGold(int amount)
    {
        gold += Mathf.Max(0, amount);
        OnChanged?.Invoke();
    }

    public bool TryGetUpgradeCosts(EquipItemSO item, out int needCopies, out int needGold)
    {
        needCopies = 0;
        needGold = 0;

        if (item == null) return false;

        var s = GetState(item.Id);
        if (s.level >= maxLevel) return false;

        needCopies = baseCopiesNeed * s.level;
        needGold = baseGoldNeed * s.level;
        return true;
    }

    public bool CanUpgrade(EquipItemSO item, out int needCopies, out int needGold)
    {
        if (!TryGetUpgradeCosts(item, out needCopies, out needGold))
            return false;

        var s = GetState(item.Id);
        return s.copies >= needCopies && gold >= needGold;
    }

    public bool TryUpgrade(EquipItemSO item)
    {
        if (!CanUpgrade(item, out int needCopies, out int needGold))
            return false;

        var s = GetState(item.Id);

        s.copies -= needCopies;
        gold -= needGold;
        s.level = Mathf.Min(s.level + 1, maxLevel);

        OnChanged?.Invoke();
        return true;
    }
}

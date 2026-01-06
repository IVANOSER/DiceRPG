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

    // Тепер золото йде в глобальний кошельок
    public void AddGold(int amount)
    {
        if (CurrencyWallet.Instance == null) return;

        CurrencyWallet.Instance.AddGold(Mathf.Max(0, amount));
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

        var walletGold = CurrencyWallet.Instance != null ? CurrencyWallet.Instance.Gold : 0;
        return s.copies >= needCopies && walletGold >= needGold;
    }

    public bool TryUpgrade(EquipItemSO item)
    {
        if (!CanUpgrade(item, out int needCopies, out int needGold))
            return false;

        if (CurrencyWallet.Instance == null) return false;

        // Спочатку пробуємо списати золото з кошелька
        if (!CurrencyWallet.Instance.TrySpendGold(needGold))
            return false;

        var s = GetState(item.Id);

        s.copies -= needCopies;
        s.level = Mathf.Min(s.level + 1, maxLevel);

        OnChanged?.Invoke();
        return true;
    }
    private void OnEnable()
{
    TryBindWallet();
}

private void Start()
{
    // якщо Manager з'явився раніше за Wallet — дочекаємось
    TryBindWallet();
    OnChanged?.Invoke(); // щоб UI одразу перерахувався
}

private void TryBindWallet()
{
    if (CurrencyWallet.Instance == null) return;

    CurrencyWallet.Instance.OnGoldChanged -= HandleGoldChanged; // щоб не було дубля
    CurrencyWallet.Instance.OnGoldChanged += HandleGoldChanged;

    CurrencyWallet.Instance.OnGemsChanged -= HandleGemsChanged;
    CurrencyWallet.Instance.OnGemsChanged += HandleGemsChanged;
}

private void OnDisable()
{
    if (CurrencyWallet.Instance == null) return;

    CurrencyWallet.Instance.OnGoldChanged -= HandleGoldChanged;
    CurrencyWallet.Instance.OnGemsChanged -= HandleGemsChanged;
}

private void HandleGoldChanged(int _)
{
    OnChanged?.Invoke(); // ОЦЕ ВАЖЛИВО
}

private void HandleGemsChanged(int _)
{
    OnChanged?.Invoke();
}

}

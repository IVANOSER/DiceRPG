using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EquipItemListEvent : UnityEvent<List<EquipItemSO>> { }

public class ChestShopPanel : MonoBehaviour
{
    [Header("Resources paths (without .json)")]
    [SerializeField] private string chestConfigPath = "Data/Configs/chest_config";
    [SerializeField] private string qualityWeightsPath = "Data/Configs/quality_weights";

    [Header("Items DB Resources path")]
    [SerializeField] private string itemsResourcesPath = "Data/Item";

    [Header("UI")]
    [SerializeField] private TMP_Text price1Text;
    [SerializeField] private TMP_Text price10Text;

    [Header("Callbacks")]
    public EquipItemListEvent OnItemsRolled;

    private ChestConfigJson chest;
    private QualityWeightsJson weights;

    private void Awake()
    {
        if (!ChestLootSystem.TryLoad(chestConfigPath, qualityWeightsPath, out chest, out weights))
        {
            Debug.LogError("[ChestShopPanel] Failed to load configs.");
            return;
        }

        if (price1Text) price1Text.text = chest.priceOpen1.ToString();
        if (price10Text) price10Text.text = chest.priceOpen10.ToString();
    }

    public void Open1()  => OpenInternal(1, chest.priceOpen1);
    public void Open10() => OpenInternal(10, chest.priceOpen10);

    private void OpenInternal(int count, int priceGems)
    {
        if (CurrencyWallet.Instance == null)
        {
            Debug.LogError("CurrencyWallet.Instance is null");
            return;
        }

        if (!CurrencyWallet.Instance.TrySpendGems(priceGems))
        {
            Debug.Log("Not enough gems");
            return;
        }

        var items = ChestLootSystem.RollItemsByQuality(weights, itemsResourcesPath, count);

// DEBUG LOG — що випало
if (items != null && items.Count > 0)
{
    Debug.Log($"[CHEST OPEN x{count}] Dropped:");
    foreach (var it in items)
    {
        if (it == null) continue;
        Debug.Log($" - {it.Id} | {it.quality} | slot: {it.slot}");
    }
}
else
{
    Debug.Log($"[CHEST OPEN x{count}] Dropped NOTHING");
}

OnItemsRolled?.Invoke(items);

    }
}

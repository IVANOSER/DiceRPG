using UnityEngine;

public class GrantAllItemsFromData : MonoBehaviour
{
    [Header("Source")]
    [Tooltip("Resources path with EquipItemSO")]
    public string itemsResourcesPath = "Data/Items";

    [Header("Refs")]
    public EquipTabController equipTabController;
    public ItemUpgradeManager upgradeManager;

    [Header("Grant")]
    public int copiesPerItem = 1;

    private void Awake()
    {
        if (upgradeManager == null)
            upgradeManager = ItemUpgradeManager.Instance;
    }

    /// <summary>
    /// Button / debug action:
    /// - gives ALL items from Data
    /// - if item already exists → only adds copy
    /// </summary>
    public void GrantAll()
    {
        if (equipTabController == null)
        {
            Debug.LogWarning("[GrantAllItemsFromData] EquipTabController is NULL");
            return;
        }

        if (upgradeManager == null)
        {
            Debug.LogWarning("[GrantAllItemsFromData] ItemUpgradeManager is NULL");
            return;
        }

        var items = Resources.LoadAll<EquipItemSO>(itemsResourcesPath);

        if (items == null || items.Length == 0)
        {
            Debug.LogWarning($"[GrantAllItemsFromData] No items found in Resources/{itemsResourcesPath}");
            return;
        }

        int granted = 0;

        foreach (var item in items)
        {
            if (item == null) continue;

            // 1️⃣ Завжди додаємо копію
            upgradeManager.AddCopies(item, Mathf.Max(1, copiesPerItem));

            // 2️⃣ В інвентар — тільки якщо ще нема
            equipTabController.AddToInventory(item);

            granted++;
        }

        Debug.Log($"[GrantAllItemsFromData] Granted {granted} items (+{copiesPerItem} copy each)");
    }
}

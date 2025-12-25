using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomEquipAllSlotsButton : MonoBehaviour
{
    [Header("Refs")]
    public CharacterMeshSwapper meshSwapper;
    public EquipTabController equipTab; // <-- щоб додавати в інвентар + оновити UI
    public Button button;

    [Header("Resources path (without Resources/)")]
    public string itemsPath = "Data/Items";

    private EquipItemSO[] pool;
    private readonly Dictionary<EquipmentSlot, List<EquipItemSO>> bySlot = new();

    


    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(EquipRandomToAllSlots);

        LoadAndIndexItems();
    }

    private void LoadAndIndexItems()
    {
        pool = Resources.LoadAll<EquipItemSO>(itemsPath);

        bySlot.Clear();
        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
            bySlot[slot] = new List<EquipItemSO>();

        if (pool == null || pool.Length == 0)
        {
            Debug.LogWarning($"[RandomEquip] No items found at Resources/{itemsPath}. Put EquipItemSO in Assets/Resources/{itemsPath}/");
            return;
        }

        foreach (var item in pool)
        {
            if (item == null) continue;
            bySlot[item.slot].Add(item);
        }
    }

    private EquipItemSO PickRandom(EquipmentSlot slot)
    {
        if (!bySlot.TryGetValue(slot, out var list) || list == null || list.Count == 0)
            return null;

        return list[Random.Range(0, list.Count)];
    }

    private void GiveAndEquip(EquipmentSlot slot)
    {
        var item = PickRandom(slot);
        if (item == null) return;

        // додаємо в інвентар (ItemPicker буде його показувати)
        if (equipTab != null)
            equipTab.AddToInventory(item);

        // одягаємо
        meshSwapper.loadout.Set(slot, item);
    }

    public void EquipRandomToAllSlots()
{
    if (meshSwapper == null)
    {
        Debug.LogWarning("[RandomEquip] meshSwapper is not assigned.");
        return;
    }

    if (meshSwapper.loadout == null)
    {
        Debug.LogWarning("[RandomEquip] meshSwapper.loadout is null. Assign PlayerLoadoutSO.");
        return;
    }

    if (pool == null || pool.Length == 0) LoadAndIndexItems();

    void GiveAndEquip(EquipmentSlot slot)
    {
        var item = PickRandom(slot);
        if (item == null) return;

        
        if (equipTab != null)
            equipTab.AddToInventory(item);

        meshSwapper.loadout.Set(slot, item);
    }

    GiveAndEquip(EquipmentSlot.RightHand);
    GiveAndEquip(EquipmentSlot.LeftHand);
    GiveAndEquip(EquipmentSlot.Helmet);
    GiveAndEquip(EquipmentSlot.Chest);
    GiveAndEquip(EquipmentSlot.Legs);
    GiveAndEquip(EquipmentSlot.Belt);

    meshSwapper.Apply();

    
    if (equipTab != null)
        equipTab.RefreshAll();

    Debug.Log("[RandomEquip] Equipped random items to all slots + added to inventory.");
}

}

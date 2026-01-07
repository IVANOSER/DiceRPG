using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipTabController : MonoBehaviour
{
    [Header("Data")]
    public PlayerLoadoutSO loadout;
    public List<EquipItemSO> allItems = new();

    [Header("Character Systems")]
    public CharacterMeshSwapper meshSwapper;
    public PlayerStats stats;

    [Header("UI Stats")]
    public Text hpText;
    public Text dmgText;

    [Header("UI Slots")]
    public EquipSlotButton[] slotButtons;

    [Header("Item Picker (Grid)")]
    public GameObject pickerPanel;
    public Transform gridContent;
    public ItemCellView itemCellPrefab;
    public Text pickerTitle;
    public Button btnClosePicker;

    [Header("Item Picker Actions")]
    public Button btnRemove;

    [Header("Item Card (Popup)")]
    public ItemCardPanelUI itemCardPanel;

    [Header("Auto load items (optional)")]
    public bool autoLoadItemsFromResources = true;
    public string itemsResourcesPath = "Data/Items";

    [Header("Dice Preview (Lobby)")]
    public DicePreviewBinder dicePreviewBinder;

    [Header("Upgrade")]
    public ItemUpgradeManager upgradeManager; // якщо не заданий — візьме Instance

    private readonly List<ItemCellView> spawnedCells = new();
    private EquipmentSlot currentSlot;

    private void Start()
    {
        Debug.Log("[EquipTabController] START OK");

        if (dicePreviewBinder == null)
            dicePreviewBinder = FindFirstObjectByType<DicePreviewBinder>(FindObjectsInactive.Include);

        if (upgradeManager == null)
            upgradeManager = ItemUpgradeManager.Instance;

        // ✅ автоматичний рефреш, коли додаються копії/голд/апгрейд
        if (upgradeManager != null)
            upgradeManager.OnChanged += RefreshAll;

        foreach (var s in slotButtons)
            if (s != null) s.Init(this);

        if (btnClosePicker) btnClosePicker.onClick.AddListener(ClosePicker);

        if (btnRemove)
            btnRemove.onClick.AddListener(RemoveFromCurrentSlot);

        if (autoLoadItemsFromResources && (allItems == null || allItems.Count == 0))
        {
            allItems = new List<EquipItemSO>(Resources.LoadAll<EquipItemSO>(itemsResourcesPath));
            Debug.Log($"[EquipTabController] Auto-loaded {allItems.Count} items from Resources/{itemsResourcesPath}");
        }

        RefreshAll();
        ClosePicker();

        Invoke(nameof(RefreshDiceFromEquipped), 0f);
    }

    private void OnDestroy()
    {
        if (upgradeManager != null)
            upgradeManager.OnChanged -= RefreshAll;
    }

    private void OnEnable()
    {
        Invoke(nameof(RefreshDiceFromEquipped), 0f);
    }

    public void OpenSlot(EquipmentSlot slot)
    {
        currentSlot = slot;

        if (pickerTitle) pickerTitle.text = slot.ToString();

        ClearGrid();

        var candidates = allItems.FindAll(i => i != null && i.slot == slot);

        foreach (var item in candidates)
        {
            var cell = Instantiate(itemCellPrefab, gridContent);
            spawnedCells.Add(cell);

            cell.Bind(item, picked =>
            {
                if (itemCardPanel == null)
                {
                    EquipPickedToCurrentSlot(picked);
                    return;
                }

                itemCardPanel.Show(
                    picked,
                    onEquipCb: EquipPickedToCurrentSlot,
                    onUpgradeCb: UpgradePickedItem
                );
            });
        }

        UpdateRemoveButtonState();

        if (pickerPanel) pickerPanel.SetActive(true);
    }

    private void EquipPickedToCurrentSlot(EquipItemSO picked)
    {
        if (loadout == null || picked == null) return;

        loadout.Set(currentSlot, picked);
        RefreshDiceFromEquipped();

        if (meshSwapper != null)
            meshSwapper.Apply();

        RefreshAll();
        ClosePicker();
    }

    private void UpgradePickedItem(EquipItemSO picked)
    {
        if (picked == null) return;

        if (upgradeManager == null)
            upgradeManager = ItemUpgradeManager.Instance;

        if (upgradeManager == null) return;

        // ✅ Реальний апгрейд (спише копії/голд, підніме lvl, OnChanged сам зробить RefreshAll)
        upgradeManager.TryUpgrade(picked);
    }

    private void RemoveFromCurrentSlot()
    {
        if (loadout == null) return;

        loadout.Set(currentSlot, null);

        if (meshSwapper != null)
            meshSwapper.Apply();

        RefreshAll();
        UpdateRemoveButtonState();
        RefreshDiceFromEquipped();
    }

    private void UpdateRemoveButtonState()
    {
        if (!btnRemove || loadout == null) return;

        var equipped = loadout.Get(currentSlot);
        btnRemove.interactable = equipped != null;
    }

    public void ClosePicker()
    {
        if (pickerPanel) pickerPanel.SetActive(false);
    }

    public void RefreshAll()
    {
        if (loadout == null) return;

        if (stats != null)
        {
            stats.Recalculate(loadout);
            if (hpText) hpText.text = stats.HP.ToString();
            if (dmgText) dmgText.text = stats.Damage.ToString();
        }

        if (upgradeManager == null)
            upgradeManager = ItemUpgradeManager.Instance;

        foreach (var s in slotButtons)
        {
            if (s == null) continue;

            var item = loadout.Get(s.slot);
            s.SetIcon(item != null ? item.icon : null);

            // ✅ ВАЖЛИВО: завжди стартуємо з false щоб бейдж не "залипав"
            bool canUpgrade = false;

            if (item != null && upgradeManager != null)
                canUpgrade = upgradeManager.CanUpgrade(item, out _, out _);

            s.SetUpgradeAvailable(canUpgrade);
        }
    }

    private void ClearGrid()
    {
        foreach (var c in spawnedCells)
            if (c != null) Destroy(c.gameObject);

        spawnedCells.Clear();
    }

    public void AddToInventory(EquipItemSO item)
    {
        if (item == null) return;
        if (allItems == null) allItems = new List<EquipItemSO>();

        if (!allItems.Contains(item))
            allItems.Add(item);
    }

    public void ClearAllEquipmentAndInventory()
    {
        if (loadout == null) return;

        loadout.Set(EquipmentSlot.RightHand, null);
        loadout.Set(EquipmentSlot.LeftHand, null);
        loadout.Set(EquipmentSlot.Helmet, null);
        loadout.Set(EquipmentSlot.Chest, null);
        loadout.Set(EquipmentSlot.Legs, null);
        loadout.Set(EquipmentSlot.Belt, null);

        if (allItems != null) allItems.Clear();

        ClearGrid();
        UpdateRemoveButtonState();
        ClosePicker();

        if (meshSwapper != null)
            meshSwapper.Apply();

        RefreshAll();
    }

    private void RefreshDiceFromEquipped()
    {
        var rt = DiceLoadoutRuntime.Instance;
        if (rt == null) return;

        SkillSO[] skills6 = new SkillSO[6]
        {
            loadout.Get(EquipmentSlot.RightHand)?.skill,
            loadout.Get(EquipmentSlot.LeftHand)?.skill,
            loadout.Get(EquipmentSlot.Belt)?.skill,
            loadout.Get(EquipmentSlot.Helmet)?.skill,
            loadout.Get(EquipmentSlot.Chest)?.skill,
            loadout.Get(EquipmentSlot.Legs)?.skill
        };

        rt.RebuildSkillFacesFromEquipped(skills6);
        dicePreviewBinder?.ApplyFromRuntime();
    }

    // ✅ викликається з badge button
    public void OpenEquippedItemCard(EquipmentSlot slot)
    {
        if (loadout == null || itemCardPanel == null) return;

        var item = loadout.Get(slot);
        if (item == null) return;

        currentSlot = slot;

        itemCardPanel.Show(
            item,
            onEquipCb: EquipPickedToCurrentSlot,
            onUpgradeCb: UpgradePickedItem
        );
    }

    public void AddItemFromChest(EquipItemSO item, int copies = 1)
{
    if (item == null) return;

    if (allItems == null) allItems = new List<EquipItemSO>();

    // 1) Додати в інвентар (лише якщо ще нема)
    if (!allItems.Contains(item))
        allItems.Add(item);

    // 2) Додати копії для апгрейду (саме так у тебе рахується можливість апгрейду)
    if (upgradeManager == null)
        upgradeManager = ItemUpgradeManager.Instance;

    if (upgradeManager != null)
        upgradeManager.AddCopies(item, Mathf.Max(1, copies));

    // 3) Оновити UI
    RefreshAll();

    // 4) Якщо пікер зараз відкритий — перебудувати грід, щоб новий айтем одразу зʼявився
    if (pickerPanel != null && pickerPanel.activeSelf)
        OpenSlot(currentSlot);
}

}

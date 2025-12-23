using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EquipTabController : MonoBehaviour
{
    [Header("Data")]
    public PlayerLoadoutSO loadout;
    public List<EquipItemSO> allItems = new();

    [Header("Character")]
    public ArmsAssembler arms;
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

    private readonly List<ItemCellView> spawnedCells = new();


    private EquipmentSlot currentSlot;
    private readonly List<Button> spawnedButtons = new();

    private void Start()
    {
        foreach (var s in slotButtons)
            s.Init(this);

        if (btnClosePicker) btnClosePicker.onClick.AddListener(ClosePicker);

        RefreshAll();
        ClosePicker();
    }

    public void OpenSlot(EquipmentSlot slot)
{
    currentSlot = slot;

    // заголовок
    if (pickerTitle) pickerTitle.text = slot.ToString();

    // очистка
    foreach (var c in spawnedCells) if (c) Destroy(c.gameObject);
    spawnedCells.Clear();

    // фільтр по слоту
    var candidates = allItems.FindAll(i => i != null && i.slot == slot);
    if (candidates.Count == 0)
    {
        Debug.LogWarning($"No items for slot: {slot}", this);
        return;
    }

    // спавнимо плитки
    foreach (var item in candidates)
    {
        var cell = Instantiate(itemCellPrefab, gridContent);
        spawnedCells.Add(cell);

        cell.Bind(item, picked =>
        {
            loadout.Set(slot, picked);
            RefreshAll();
            ClosePicker();
        });
    }

    pickerPanel.SetActive(true);
}


    public void ClosePicker()
    {
        if (pickerPanel) pickerPanel.SetActive(false);
    }

    private void RefreshAll()
    {
        // застосувати руки
        if (arms != null)
        {
            var left = loadout.leftHand;
            if (left != null && left.isArmPart)
                arms.SetLeftArm(left.armVariantIndex);

            var right = loadout.rightHand;
            if (right != null && right.isArmPart)
                arms.SetRightArm(right.armVariantIndex);
        }

        // стати
        if (stats != null)
        {
            stats.Recalculate(loadout);
            if (hpText) hpText.text = stats.HP.ToString();
            if (dmgText) dmgText.text = stats.Damage.ToString();
        }

        // іконки слотів
        foreach (var s in slotButtons)
        {
            var item = loadout.Get(s.slot);
            s.SetIcon(item != null ? item.icon : null);
        }
    }
}

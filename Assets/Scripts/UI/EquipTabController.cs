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
    public Button btnRemove; // ✅ нова кнопка "зняти"

    [Header("Auto load items (optional)")]
    public bool autoLoadItemsFromResources = true;
    public string itemsResourcesPath = "Data/Items";

    [Header("Dice Preview (Lobby)")]
    public DicePreviewBinder dicePreviewBinder; // перетягнеш в інспекторі (з лоббі сцени)


    private readonly List<ItemCellView> spawnedCells = new();
    private EquipmentSlot currentSlot;

    private void Start()
{
    Debug.Log("[EquipTabController] START OK");

    if (dicePreviewBinder == null)
        dicePreviewBinder = FindObjectOfType<DicePreviewBinder>(true);

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

    // важливо: на наступний кадр
    Invoke(nameof(RefreshDiceFromEquipped), 0f);
}

    private void OnEnable()
    {
    // коли повернувся в Equip таб
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
                loadout.Set(slot, picked);
                RefreshDiceFromEquipped();


                if (meshSwapper != null)
                    meshSwapper.Apply();

                RefreshAll();
                ClosePicker();
            });
        }

        UpdateRemoveButtonState();

        if (pickerPanel) pickerPanel.SetActive(true);
        
    }

    private void RemoveFromCurrentSlot()
    {
        if (loadout == null) return;

        // зняти айтем
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

        // активна, якщо в слоті щось одягнено
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

        foreach (var s in slotButtons)
        {
            if (s == null) continue;
            var item = loadout.Get(s.slot);
            s.SetIcon(item != null ? item.icon : null);
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

    // щоб не дублювати один і той самий SO
    if (!allItems.Contains(item))
        allItems.Add(item);
}

public void ClearAllEquipmentAndInventory()
{
    if (loadout == null) return;

    // зняти з усіх слотів
    loadout.Set(EquipmentSlot.RightHand, null);
    loadout.Set(EquipmentSlot.LeftHand,  null);
    loadout.Set(EquipmentSlot.Helmet,    null);
    loadout.Set(EquipmentSlot.Chest,     null);
    loadout.Set(EquipmentSlot.Legs,      null);
    loadout.Set(EquipmentSlot.Belt,      null);

    // очистити інвентар (це і є контент ItemPicker)
    if (allItems != null) allItems.Clear();

    // закрити/почистити пікер
    ClearGrid();
    UpdateRemoveButtonState();
    ClosePicker();

    // оновити персонажа
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

    // 🔥 ОДИН ЄДИНИЙ ВИКЛИК
    dicePreviewBinder?.ApplyFromRuntime();
}



}

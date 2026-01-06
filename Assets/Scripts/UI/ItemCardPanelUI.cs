using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardPanelUI : MonoBehaviour
{
    [Header("Main UI")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text skillsText;

    [Header("Level / Upgrade UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text costText;

    [Header("Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;

    [Header("Refs")]
    [SerializeField] private ItemUpgradeManager upgradeManager;

    private EquipItemSO currentItem;
    private Action<EquipItemSO> onEquip;
    private Action<EquipItemSO> onUpgrade;

    private bool initialized;

    private void Awake()
    {
        EnsureInit();
        // ❗ НЕ ховаємо панель тут. Ховай її в інспекторі (SetActive(false)) або вручну ззовні.
    }

    private void EnsureInit()
    {
        if (initialized) return;
        initialized = true;

        if (equipButton) equipButton.onClick.AddListener(HandleEquip);
        if (upgradeButton) upgradeButton.onClick.AddListener(HandleUpgrade);
        if (closeButton) closeButton.onClick.AddListener(Hide);
    }

    public void Show(EquipItemSO item, Action<EquipItemSO> onEquipCb, Action<EquipItemSO> onUpgradeCb)
    {
        EnsureInit();

        currentItem = item;
        onEquip = onEquipCb;
        onUpgrade = onUpgradeCb;

        if (upgradeManager == null)
            upgradeManager = ItemUpgradeManager.Instance;

        gameObject.SetActive(true);
        Bind(item);
    }

    public void Hide()
    {
        currentItem = null;
        onEquip = null;
        onUpgrade = null;
        gameObject.SetActive(false);
    }

    private void Bind(EquipItemSO item)
    {
        if (item == null || upgradeManager == null) return;

        if (nameText) nameText.text = string.IsNullOrWhiteSpace(item.displayName) ? item.Id : item.displayName;

        if (iconImage)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = item.icon != null;
        }

        if (statsText) statsText.text = item.GetStatsTextForCard();
        if (skillsText) skillsText.text = item.GetSkillsTextForCard();

        RefreshUpgradeUI(item);
    }

    private void RefreshUpgradeUI(EquipItemSO item)
    {
        if (upgradeManager == null || item == null) return;

        var state = upgradeManager.GetState(item.Id);

        if (levelText) levelText.text = $"{state.level}";

        if (upgradeManager.TryGetUpgradeCosts(item, out int needCopies, out int needGold))
        {
            if (progressSlider)
            {
                progressSlider.minValue = 0;
                progressSlider.maxValue = needCopies;
                progressSlider.value = Mathf.Clamp(state.copies, 0, needCopies);
            }

            if (progressText) progressText.text = $"{state.copies}/{needCopies}";
            if (costText) costText.text = $"{needGold}";

            if (upgradeButton)
                upgradeButton.interactable = upgradeManager.CanUpgrade(item, out _, out _);
        }
        else
        {
            if (progressSlider)
            {
                progressSlider.minValue = 0;
                progressSlider.maxValue = 1;
                progressSlider.value = 1;
            }

            if (progressText) progressText.text = "MAX";
            if (costText) costText.text = "";
            if (upgradeButton) upgradeButton.interactable = false;
        }
    }

    private void HandleEquip()
    {
        if (currentItem == null) return;
        onEquip?.Invoke(currentItem);
        Hide();
    }

    private void HandleUpgrade()
    {
        if (currentItem == null || upgradeManager == null) return;

        if (upgradeManager.TryUpgrade(currentItem))
        {
            onUpgrade?.Invoke(currentItem);
            RefreshUpgradeUI(currentItem);
        }
    }
}

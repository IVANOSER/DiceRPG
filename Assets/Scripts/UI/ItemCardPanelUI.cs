using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCardPanelUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private TMP_Text skillsText;

    [Header("Buttons")]
    [SerializeField] private Button equipButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeButton;

    [Header("Optional: click outside to close")]
    [SerializeField] private Button backgroundCloseButton; // якщо є затемнений фон-кнопка

    private EquipItemSO currentItem;
    private Action<EquipItemSO> onEquip;
    private Action<EquipItemSO> onUpgrade;

    private void Awake()
    {
        if (equipButton) equipButton.onClick.AddListener(HandleEquip);
        if (upgradeButton) upgradeButton.onClick.AddListener(HandleUpgrade);
        if (closeButton) closeButton.onClick.AddListener(Hide);

        if (backgroundCloseButton)
            backgroundCloseButton.onClick.AddListener(Hide);

        Hide();
    }

    /// <summary>
    /// Відкрити карточку на конкретний item
    /// </summary>
    public void Show(EquipItemSO item, Action<EquipItemSO> onEquipCb, Action<EquipItemSO> onUpgradeCb)
    {
        currentItem = item;
        onEquip = onEquipCb;
        onUpgrade = onUpgradeCb;

        Bind(item);

        gameObject.SetActive(true);
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
        if (item == null)
        {
            if (nameText) nameText.text = "-";
            if (iconImage) iconImage.enabled = false;
            if (statsText) statsText.text = "";
            if (skillsText) skillsText.text = "";
            if (equipButton) equipButton.interactable = false;
            if (upgradeButton) upgradeButton.interactable = false;
            return;
        }

        if (nameText) nameText.text = string.IsNullOrWhiteSpace(item.displayName) ? item.name : item.displayName;

        if (iconImage)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = item.icon != null;
        }

        if (statsText) statsText.text = item.GetStatsTextForCard();
        if (skillsText) skillsText.text = item.GetSkillsTextForCard();

        if (equipButton) equipButton.interactable = true;
        if (upgradeButton) upgradeButton.interactable = true; // потім можна буде блокати, якщо не можна апгрейдити
    }

    private void HandleEquip()
    {
        if (currentItem == null) return;

        onEquip?.Invoke(currentItem);
        Hide();
    }

    private void HandleUpgrade()
    {
        if (currentItem == null) return;

        onUpgrade?.Invoke(currentItem);
        // апгрейд може не закривати — залишаю відкритим
        // якщо хочеш закривати — розкоментуй:
        // Hide();
    }
}

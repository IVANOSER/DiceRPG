using UnityEngine;
using UnityEngine.UI;

public class EquipSlotButton : MonoBehaviour
{
    public EquipmentSlot slot;
    public Button button;
    public Image icon;

    [Header("Upgrade Badge Button")]
    public Button upgradeBadgeButton; // перетягни сюди Button бейджа

    private EquipTabController controller;

    public void Init(EquipTabController c)
    {
        controller = c;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => controller.OpenSlot(slot));
        }

        if (upgradeBadgeButton != null)
        {
            upgradeBadgeButton.onClick.RemoveAllListeners();
            upgradeBadgeButton.onClick.AddListener(() => controller.OpenEquippedItemCard(slot));
        }
    }

    public void SetIcon(Sprite s)
    {
        if (icon) icon.sprite = s;
        if (icon) icon.enabled = (s != null);
    }

    public void SetUpgradeAvailable(bool isAvailable)
    {
        if (upgradeBadgeButton != null)
            upgradeBadgeButton.gameObject.SetActive(isAvailable);
    }
}

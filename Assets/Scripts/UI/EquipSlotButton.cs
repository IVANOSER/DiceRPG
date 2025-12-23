using UnityEngine;
using UnityEngine.UI;

public class EquipSlotButton : MonoBehaviour
{
    public EquipmentSlot slot;
    public Button button;
    public Image icon;

    private EquipTabController controller;

    public void Init(EquipTabController c)
    {
        controller = c;
        button.onClick.AddListener(() => controller.OpenSlot(slot));
    }

    public void SetIcon(Sprite s)
    {
        if (icon) icon.sprite = s;
        if (icon) icon.enabled = (s != null);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ItemCellView : MonoBehaviour
{
    public Button button;
    public Image icon;
    public Image frame; // optional

    private EquipItemSO item;
    private System.Action<EquipItemSO> onClick;

    public void Bind(EquipItemSO newItem, System.Action<EquipItemSO> click)
    {
        item = newItem;
        onClick = click;

        if (icon)
        {
            icon.sprite = item.icon;
            icon.enabled = item.icon != null;
        }

        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(item));
        }
    }
}

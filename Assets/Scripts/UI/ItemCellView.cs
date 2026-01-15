using UnityEngine;
using UnityEngine.UI;

public class ItemCellView : MonoBehaviour
{
    public Button button;
    public Image icon;
    public Image frame; // optional

    [Header("Quality Colors")]
    public Color goodColor = Color.green;
    public Color bttrColor = Color.blue;
    public Color rareColor = new Color(0.6f, 0.2f, 0.8f);     // фіолетовий
    public Color legColor  = new Color(1f, 0.6f, 0.1f);       // золотистий/оранжевий
    public Color defaultColor = Color.white;

    private UltimateSO ultimate;
    private System.Action<UltimateSO> onUltimateClick;

    private EquipItemSO item;
    private System.Action<EquipItemSO> onClick;

    public void Bind(EquipItemSO newItem, System.Action<EquipItemSO> click)
    {
        item = newItem;
        onClick = click;

        if (icon)
        {
            icon.sprite = item != null ? item.icon : null;
            icon.enabled = item != null && item.icon != null;
        }

        if (frame)
        {
            ApplyQualityColor(item);
        }

        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClick?.Invoke(item));
        }
    }

    private void ApplyQualityColor(EquipItemSO it)
    {
        if (it == null)
        {
            frame.color = defaultColor;
            return;
        }

        switch (it.quality)
        {
            case ItemQuality.good:
                frame.color = goodColor;
                break;
            case ItemQuality.bttr:
                frame.color = bttrColor;
                break;
            case ItemQuality.rare:
                frame.color = rareColor;
                break;
            case ItemQuality.leg:
                frame.color = legColor;
                break;
            default:
                frame.color = defaultColor;
                break;
        }
    }

    public void Bind(UltimateSO newUltimate, System.Action<UltimateSO> click)
    {
        ultimate = newUltimate;
        onUltimateClick = click;

        // icon
        if (icon)
        {
            icon.sprite = ultimate != null ? ultimate.icon : null;
            icon.enabled = ultimate != null && ultimate.icon != null;
        }

        // frame — для ульти просто дефолтний (або можна по coreColor)
        if (frame)
        {
            frame.color = defaultColor;
        }

        if (button)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onUltimateClick?.Invoke(ultimate));
        }
    }
}

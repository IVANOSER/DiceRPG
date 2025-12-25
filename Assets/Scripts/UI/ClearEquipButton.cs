using UnityEngine;
using UnityEngine.UI;

public class ClearAllEquipButton : MonoBehaviour
{
    public EquipTabController equipTab;
    public Button button;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        button.onClick.AddListener(() => equipTab.ClearAllEquipmentAndInventory());
    }
}

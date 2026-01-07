using System.Collections.Generic;
using UnityEngine;

public class ChestRewardReceiver : MonoBehaviour
{
    [SerializeField] private EquipTabController equipTab;

    // Викликається з ChestShopPanel.OnItemsRolled (List<EquipItemSO>)
    public void Receive(List<EquipItemSO> items)
    {
        if (equipTab == null)
        {
            Debug.LogError("[ChestReceiver] EquipTabController not assigned");
            return;
        }

        foreach (var item in items)
        {
            if (item == null) continue;
            equipTab.AddItemFromChest(item, copies: 1);
        }
    }
}

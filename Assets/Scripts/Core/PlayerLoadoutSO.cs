using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/Loadout")]
public class PlayerLoadoutSO : ScriptableObject
{
    public EquipItemSO leftHand, rightHand, belt, helmet, chest, legs;

    public EquipItemSO Get(EquipmentSlot slot) => slot switch
    {
        EquipmentSlot.LeftHand => leftHand,
        EquipmentSlot.RightHand => rightHand,
        EquipmentSlot.Belt => belt,
        EquipmentSlot.Helmet => helmet,
        EquipmentSlot.Chest => chest,
        EquipmentSlot.Legs => legs,
        _ => null
    };

    public void Set(EquipmentSlot slot, EquipItemSO item)
    {
        switch (slot)
        {
            case EquipmentSlot.LeftHand: leftHand = item; break;
            case EquipmentSlot.RightHand: rightHand = item; break;
            case EquipmentSlot.Belt: belt = item; break;
            case EquipmentSlot.Helmet: helmet = item; break;
            case EquipmentSlot.Chest: chest = item; break;
            case EquipmentSlot.Legs: legs = item; break;
        }
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Game/Equipment/Equip Item")]
public class EquipItemSO : ScriptableObject
{
    public string id;
    public string displayName;
    public EquipmentSlot slot;
    public StatModifier[] modifiers;

    public Sprite icon;
    public GameObject prefab3D;

    [Header("Body Part Swap")]
    public bool isArmPart;
    public int armVariantIndex;

}

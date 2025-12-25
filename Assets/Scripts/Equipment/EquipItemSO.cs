using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Equipment/Equip Item")]
public class EquipItemSO : ScriptableObject
{
    [Header("Setup (dropdowns)")]
    public ItemQuality quality = ItemQuality.good;
    public EquipmentSlot slot = EquipmentSlot.Belt;

    [Tooltip("Only this is typed manually. Example: 1 -> 001")]
    [Min(1)]
    public int setNumber = 1;

    [Header("Auto-generated ID")]
    [SerializeField, Tooltip("Auto: quality.type.set (e.g. bttr.bl.001)")]
    private string id;
    public string Id => id;

    [Header("UI")]
    public string displayName;
    public Sprite icon;

    [Header("Stats")]
    public StatModifier[] modifiers;

    [Header("Synty Mesh Swap (body parts)")]
    public List<MeshReplace> meshReplaces = new();

    [Serializable]
    public class MeshReplace
    {
        [Tooltip("Which character mesh slot to replace (e.g. ArmUpperRight)")]
        public BodyPartSlot target;

        [Tooltip("Mesh asset to assign into SkinnedMeshRenderer.sharedMesh")]
        public Mesh mesh;

        [Tooltip("Optional: override material for this part (usually you can leave empty)")]
        public Material materialOverride;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (setNumber < 1) setNumber = 1;

        id = BuildId(quality, slot, setNumber);

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = id;
    }
#endif

    public static string BuildId(ItemQuality q, EquipmentSlot s, int setNum)
    {
        return $"{q}.{SlotToTypeCode(s)}.{setNum:000}";
    }

    private static string SlotToTypeCode(EquipmentSlot s)
    {
        return s switch
        {
            EquipmentSlot.Belt => "bl",
            EquipmentSlot.Legs => "lg",
            EquipmentSlot.RightHand => "rhd",
            EquipmentSlot.LeftHand => "lhd",
            EquipmentSlot.Helmet => "hd",
            EquipmentSlot.Chest => "ch",
            _ => "bl"
        };
    }
}

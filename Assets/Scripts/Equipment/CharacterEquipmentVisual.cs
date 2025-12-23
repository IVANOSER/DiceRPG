using System.Collections.Generic;
using UnityEngine;

public class CharacterEquipmentVisual : MonoBehaviour
{
    public Transform slotLeftHand, slotRightHand, slotHead, slotChest, slotLegs, slotBelt;

    private readonly Dictionary<EquipmentSlot, GameObject> spawned = new();

    public void ApplyLoadout(PlayerLoadoutSO loadout)
    {
        Apply(EquipmentSlot.LeftHand, loadout.leftHand);
        Apply(EquipmentSlot.RightHand, loadout.rightHand);
        Apply(EquipmentSlot.Helmet, loadout.helmet);
        Apply(EquipmentSlot.Chest, loadout.chest);
        Apply(EquipmentSlot.Legs, loadout.legs);
        Apply(EquipmentSlot.Belt, loadout.belt);
    }

    void Apply(EquipmentSlot slot, EquipItemSO item)
    {
        if (spawned.TryGetValue(slot, out var old) && old) Destroy(old);
        spawned[slot] = null;

        if (item == null || item.prefab3D == null) return;

        var parent = GetParent(slot);
        if (!parent) return;

        var go = Instantiate(item.prefab3D, parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        spawned[slot] = go;
    }

    Transform GetParent(EquipmentSlot slot) => slot switch
    {
        EquipmentSlot.LeftHand => slotLeftHand,
        EquipmentSlot.RightHand => slotRightHand,
        EquipmentSlot.Helmet => slotHead,
        EquipmentSlot.Chest => slotChest,
        EquipmentSlot.Legs => slotLegs,
        EquipmentSlot.Belt => slotBelt,
        _ => null
    };
}

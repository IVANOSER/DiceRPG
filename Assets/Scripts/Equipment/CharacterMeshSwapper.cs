using System.Collections.Generic;
using UnityEngine;

public class CharacterMeshSwapper : MonoBehaviour
{
    public PlayerLoadoutSO loadout;
    public CharacterMeshSlots meshSlots;

    // Defaults for skinned parts
    private readonly Dictionary<BodyPartSlot, Mesh> defaultMeshes = new();
    private readonly Dictionary<BodyPartSlot, Material> defaultMaterials = new();

    // Defaults for weapon (non-skinned)
    private readonly Dictionary<BodyPartSlot, Mesh> defaultWeaponMeshes = new();
    private readonly Dictionary<BodyPartSlot, Material> defaultWeaponMaterials = new();

    private void Awake()
    {
        if (!meshSlots) meshSlots = GetComponent<CharacterMeshSlots>();
        CacheDefaults();
    }

    private void Start()
    {
        Apply();
    }

    private static bool IsWeaponSlot(BodyPartSlot slot)
        => slot == BodyPartSlot.WeaponRight || slot == BodyPartSlot.WeaponLeft;

    private void CacheDefaults()
    {
        if (meshSlots == null) return;

        foreach (BodyPartSlot slot in System.Enum.GetValues(typeof(BodyPartSlot)))
        {
            if (IsWeaponSlot(slot))
            {
                var mf = meshSlots.GetWeapon(slot);
                if (mf == null) continue;

                defaultWeaponMeshes[slot] = mf.sharedMesh;

                // Material for weapon is on MeshRenderer
                var mr = mf.GetComponent<MeshRenderer>();
                if (mr != null) defaultWeaponMaterials[slot] = mr.sharedMaterial;

                continue;
            }

            var smr = meshSlots.Get(slot);
            if (smr == null) continue;

            defaultMeshes[slot] = smr.sharedMesh;
            defaultMaterials[slot] = smr.sharedMaterial;
        }
    }

    public void Apply()
    {
        if (meshSlots == null) return;

        ResetAllToDefault();
        if (loadout == null) return;

        ApplyItem(loadout.leftHand);
        ApplyItem(loadout.rightHand);
        ApplyItem(loadout.helmet);
        ApplyItem(loadout.chest);
        ApplyItem(loadout.belt);
        ApplyItem(loadout.legs);
    }

    private void ResetAllToDefault()
    {
        // Reset skinned parts
        foreach (var kv in defaultMeshes)
        {
            var smr = meshSlots.Get(kv.Key);
            if (smr == null) continue;

            smr.sharedMesh = kv.Value;

            if (defaultMaterials.TryGetValue(kv.Key, out var mat) && mat != null)
                smr.sharedMaterial = mat;
        }

        // Reset weapons (MeshFilter + MeshRenderer)
        foreach (var kv in defaultWeaponMeshes)
        {
            var mf = meshSlots.GetWeapon(kv.Key);
            if (mf == null) continue;

            mf.sharedMesh = kv.Value;

            var mr = mf.GetComponent<MeshRenderer>();
            if (mr != null && defaultWeaponMaterials.TryGetValue(kv.Key, out var mat) && mat != null)
                mr.sharedMaterial = mat;
        }
    }

    private void ApplyItem(EquipItemSO item)
    {
        if (item == null || item.meshReplaces == null) return;

        foreach (var r in item.meshReplaces)
        {
            if (r == null || r.mesh == null) continue;

            if (IsWeaponSlot(r.target))
            {
                var mf = meshSlots.GetWeapon(r.target);
                if (mf == null) continue;

                mf.sharedMesh = r.mesh;

                if (r.materialOverride != null)
                {
                    var mr = mf.GetComponent<MeshRenderer>();
                    if (mr != null) mr.sharedMaterial = r.materialOverride;
                }

                continue;
            }

            var smr = meshSlots.Get(r.target);
            if (smr == null) continue;

            smr.sharedMesh = r.mesh;

            if (r.materialOverride != null)
                smr.sharedMaterial = r.materialOverride;
        }
    }
}

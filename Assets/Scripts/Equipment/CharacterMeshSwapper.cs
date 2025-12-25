using System.Collections.Generic;
using UnityEngine;

public class CharacterMeshSwapper : MonoBehaviour
{
    public PlayerLoadoutSO loadout;
    public CharacterMeshSlots meshSlots;

    // дефолт для кожного BodyPartSlot
    private readonly Dictionary<BodyPartSlot, Mesh> defaultMeshes = new();
    private readonly Dictionary<BodyPartSlot, Material> defaultMaterials = new();

    private void Awake()
    {
        if (!meshSlots) meshSlots = GetComponent<CharacterMeshSlots>();
        CacheDefaults();
    }

    private void Start()
    {
        Apply(); // щоб на старті було правильно
    }

    private void CacheDefaults()
    {
        if (meshSlots == null) return;

        foreach (BodyPartSlot slot in System.Enum.GetValues(typeof(BodyPartSlot)))
        {
            var smr = meshSlots.Get(slot);
            if (smr == null) continue;

            defaultMeshes[slot] = smr.sharedMesh;
            defaultMaterials[slot] = smr.sharedMaterial;
        }
    }

    public void Apply()
    {
        if (meshSlots == null) return;

        // 1) Скидаємо все в дефолт
        ResetAllToDefault();

        // 2) Накладаємо екіп
        if (loadout == null) return;

        ApplyItem(loadout.leftHand);
        ApplyItem(loadout.rightHand);
        ApplyItem(loadout.helmet);
        ApplyItem(loadout.chest);
        ApplyItem(loadout.legs);
    }

    private void ResetAllToDefault()
    {
        foreach (var kv in defaultMeshes)
        {
            var smr = meshSlots.Get(kv.Key);
            if (smr == null) continue;

            smr.sharedMesh = kv.Value;

            if (defaultMaterials.TryGetValue(kv.Key, out var mat) && mat != null)
                smr.sharedMaterial = mat;
        }
    }

    private void ApplyItem(EquipItemSO item)
    {
        if (item == null || item.meshReplaces == null) return;

        foreach (var r in item.meshReplaces)
        {
            if (r == null || r.mesh == null) continue;

            var smr = meshSlots.Get(r.target);
            if (smr == null) continue;

            smr.sharedMesh = r.mesh;

            if (r.materialOverride != null)
                smr.sharedMaterial = r.materialOverride;
        }
    }
}

using UnityEngine;

public class CharacterMeshSlots : MonoBehaviour
{
    [Header("Right arm")]
    public SkinnedMeshRenderer armUpperRight;
    public SkinnedMeshRenderer armLowerRight;
    public SkinnedMeshRenderer handRight;

    // Weapons are NOT skinned. Use MeshFilter (and MeshRenderer on the same object for materials).
    public MeshFilter weaponRight;

    [Header("Left arm")]
    public SkinnedMeshRenderer armUpperLeft;
    public SkinnedMeshRenderer armLowerLeft;
    public SkinnedMeshRenderer handLeft;

    // Weapons are NOT skinned. Use MeshFilter (and MeshRenderer on the same object for materials).
    public MeshFilter weaponLeft;

    [Header("Body")]
    public SkinnedMeshRenderer head;
    public SkinnedMeshRenderer helmet;
    public SkinnedMeshRenderer cover;
    public SkinnedMeshRenderer chest;

    [Header("Belt")]
    public SkinnedMeshRenderer hips;

    [Header("Legs")]
    public SkinnedMeshRenderer legLeft;
    public SkinnedMeshRenderer legRight;



    /// <summary>
    /// Returns SkinnedMeshRenderer for body parts (NOT weapons).
    /// </summary>
    public SkinnedMeshRenderer GetSkinned(BodyPartSlot slot)
    {
        return slot switch
        {
            BodyPartSlot.ArmUpperRight => armUpperRight,
            BodyPartSlot.ArmLowerRight => armLowerRight,
            BodyPartSlot.HandRight => handRight,

            BodyPartSlot.ArmUpperLeft => armUpperLeft,
            BodyPartSlot.ArmLowerLeft => armLowerLeft,
            BodyPartSlot.HandLeft => handLeft,

            BodyPartSlot.Head => head,
            BodyPartSlot.Helmet => helmet,
            BodyPartSlot.Cover => cover,
            BodyPartSlot.Chest => chest,

            BodyPartSlot.Hips => hips,
            BodyPartSlot.LegLeft => legLeft,
            BodyPartSlot.LegRight => legRight,           

            _ => null
        };
    }

    /// <summary>
    /// Returns MeshFilter for weapons (NOT skinned).
    /// </summary>
    public MeshFilter GetWeapon(BodyPartSlot slot)
    {
        return slot switch
        {
            BodyPartSlot.WeaponRight => weaponRight,
            BodyPartSlot.WeaponLeft => weaponLeft,
            _ => null
        };
    }

    // Backward compatible name (if you used Get() everywhere for skinned parts).
    public SkinnedMeshRenderer Get(BodyPartSlot slot) => GetSkinned(slot);
}

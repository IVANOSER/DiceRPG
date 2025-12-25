using UnityEngine;

public class CharacterMeshSlots : MonoBehaviour
{
    [Header("Right arm")]
    public SkinnedMeshRenderer armUpperRight;
    public SkinnedMeshRenderer armLowerRight;
    public SkinnedMeshRenderer handRight;

    [Header("Left arm")]
    public SkinnedMeshRenderer armUpperLeft;
    public SkinnedMeshRenderer armLowerLeft;
    public SkinnedMeshRenderer handLeft;

    [Header("Body")]
    public SkinnedMeshRenderer head;     // базова голова
    public SkinnedMeshRenderer helmet;   // helmet mesh
    public SkinnedMeshRenderer chest;

    [Header("Legs")]
    public SkinnedMeshRenderer hips;
    public SkinnedMeshRenderer legLeft;
    public SkinnedMeshRenderer legRight;

    public SkinnedMeshRenderer Get(BodyPartSlot slot)
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
            BodyPartSlot.Chest => chest,

            BodyPartSlot.Hips => hips,
            BodyPartSlot.LegLeft => legLeft,
            BodyPartSlot.LegRight => legRight,

            _ => null
        };
    }
}

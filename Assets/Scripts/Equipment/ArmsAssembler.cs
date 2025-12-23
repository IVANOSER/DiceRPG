using UnityEngine;

public class ArmsAssembler : MonoBehaviour
{
    public ChildVariantGroup rightUpper;
    public ChildVariantGroup rightLower;
    public ChildVariantGroup rightHand;

    public ChildVariantGroup leftUpper;
    public ChildVariantGroup leftLower;
    public ChildVariantGroup leftHand;

    public void SetRightArm(int index)
    {
        rightUpper.SetIndex(index);
        rightLower.SetIndex(index);
        rightHand.SetIndex(index);
    }

    public void SetLeftArm(int index)
    {
        leftUpper.SetIndex(index);
        leftLower.SetIndex(index);
        leftHand.SetIndex(index);
    }
}

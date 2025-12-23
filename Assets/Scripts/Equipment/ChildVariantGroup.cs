using UnityEngine;

public class ChildVariantGroup : MonoBehaviour
{
    [Min(0)] public int defaultIndex = 0;

    [ContextMenu("Apply Default Index")]
    public void ApplyDefault() => SetIndex(defaultIndex);

    public void SetIndex(int index)
    {
        int count = transform.childCount;
        if (count == 0) return;

        index = Mathf.Clamp(index, 0, count - 1);

        for (int i = 0; i < count; i++)
            transform.GetChild(i).gameObject.SetActive(i == index);
    }
}

using UnityEngine;

public enum DieKind { SkillD12 }

[RequireComponent(typeof(Collider))]
public class DieSelectable3D : MonoBehaviour
{
    public DieKind kind;
    [SerializeField] private DiceTurnController diceTurn; // перетягнеш в інспекторі

    public void Select()
    {
        if (diceTurn == null) return;
        if (kind == DieKind.SkillD12)
            diceTurn.SelectSkillDie();
    }
}

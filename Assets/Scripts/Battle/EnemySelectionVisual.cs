using UnityEngine;

public class EnemySelectionVisual : MonoBehaviour
{
    [SerializeField] private GameObject selectionCylinder;

    private void Awake()
    {
        // якщо не призначив в ≥нспектор≥ Ч спробуЇмо знайти по ≥мен≥
        if (selectionCylinder == null)
        {
            var t = transform.Find("SelectionCylinder");
            if (t != null) selectionCylinder = t.gameObject;
        }

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionCylinder != null)
            selectionCylinder.SetActive(selected);
    }
}

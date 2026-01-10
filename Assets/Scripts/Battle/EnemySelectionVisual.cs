using UnityEngine;

public class EnemySelectionVisual : MonoBehaviour
{
    [SerializeField] private GameObject selectionCylinder;
    [Header("Pulse")]
    [SerializeField] private bool pulse = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.08f;

    private Vector3 baseScale;
    private bool selected;

    private void Awake()
    {
        if (selectionCylinder == null)
        {
            var t = transform.Find("SelectionCylinder");
            if (t != null) selectionCylinder = t.gameObject;
        }

        if (selectionCylinder != null)
            baseScale = selectionCylinder.transform.localScale;

        SetSelected(false);
    }

    private void Update()
    {
        if (!pulse || !selected || selectionCylinder == null) return;

        // пульсуємо тільки XZ, щоб “стовп” не стрибав по висоті
        float k = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        var s = baseScale;
        s.x = baseScale.x * k;
        s.z = baseScale.z * k;
        selectionCylinder.transform.localScale = s;
    }

    public void SetSelected(bool isSelected)
    {
        selected = isSelected;

        if (selectionCylinder != null)
        {
            selectionCylinder.SetActive(isSelected);
            if (!isSelected) selectionCylinder.transform.localScale = baseScale;
        }
    }
}

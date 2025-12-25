using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBarUI : MonoBehaviour
{
    [SerializeField] private EnemyHealth health;
    [SerializeField] private Slider slider;
    [SerializeField] private Transform uiRoot;

    private Camera cam;

    private void Awake()
    {
        if (health == null) health = GetComponent<EnemyHealth>();
        cam = Camera.main;
    }

    private void OnEnable()
    {
        health.OnHpChanged.AddListener(UpdateUI);
        UpdateUI(health.CurrentHp, health.MaxHp);
    }

    private void OnDisable()
    {
        health.OnHpChanged.RemoveListener(UpdateUI);
    }

    private void LateUpdate()
    {
        if (uiRoot != null && cam != null)
            uiRoot.forward = cam.transform.forward;
    }

    private void UpdateUI(int current, int max)
    {
        if (slider == null) return;
        slider.maxValue = max;
        slider.value = current;
    }
}

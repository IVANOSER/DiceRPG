using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemyHealth health;
    [SerializeField] private StatusController statusController;

    [Header("HP")]
    [SerializeField] private Slider hpSlider;

    [Header("Status Icons")]
    [SerializeField] private Image stunIcon;

    [Header("UI Root")]
    [SerializeField] private Transform uiRoot;

    private Camera cam;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<EnemyHealth>();

        if (statusController == null)
            statusController = GetComponent<StatusController>();

        cam = Camera.main;

        if (stunIcon != null)
            stunIcon.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnHpChanged.AddListener(UpdateHp);
            UpdateHp(health.CurrentHp, health.MaxHp);
        }

        if (statusController != null)
        {
            statusController.OnStatusesChanged += RefreshStatuses;
            RefreshStatuses();
        }
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnHpChanged.RemoveListener(UpdateHp);

        if (statusController != null)
            statusController.OnStatusesChanged -= RefreshStatuses;
    }

    private void LateUpdate()
    {
        if (uiRoot != null && cam != null)
            uiRoot.forward = cam.transform.forward;
    }

    // ---------- HP ----------
    private void UpdateHp(int current, int max)
    {
        if (hpSlider == null) return;

        hpSlider.maxValue = max;
        hpSlider.value = current;
    }

    // ---------- Statuses ----------
    private void RefreshStatuses()
    {
        if (stunIcon != null)
            stunIcon.gameObject.SetActive(
                statusController != null && statusController.Has<StunStatus>()
            );
    }
}

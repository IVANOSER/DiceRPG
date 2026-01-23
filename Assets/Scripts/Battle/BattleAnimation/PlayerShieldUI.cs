using UnityEngine;
using TMPro;

public class PlayerShieldUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject shieldRoot;
    public TMP_Text shieldValueText;

    [Header("Player (optional)")]
    public StatusController statusController;

    [Header("Fallback polling")]
    public bool usePollingIfNoEvents = true;
    public float pollInterval = 0.15f;

    float pollTimer;

    private void Awake()
    {
        if (statusController == null)
        {
            var ph = FindFirstObjectByType<PlayerHealth>(FindObjectsInactive.Include);
            if (ph != null) statusController = ph.GetComponent<StatusController>();
        }
    }

    private void OnEnable()
    {
        TrySubscribe();
        Refresh();
    }

    private void OnDisable()
    {
        TryUnsubscribe();
    }

    private void Update()
    {
        if (!usePollingIfNoEvents) return;

        pollTimer -= Time.deltaTime;
        if (pollTimer <= 0f)
        {
            pollTimer = pollInterval;
            Refresh();
        }
    }

    void Refresh()
    {
        if (shieldRoot == null || shieldValueText == null || statusController == null)
        {
            if (shieldRoot) shieldRoot.SetActive(false);
            return;
        }

        // ✅ СУМА всіх щитів (навіть якщо їх кілька)
        int total = 0;
        var shields = statusController.GetAll<ShieldStatus>();
        for (int i = 0; i < shields.Count; i++)
            total += shields[i].AbsorbsLeft;

        if (total > 0)
        {
            shieldRoot.SetActive(true);
            shieldValueText.text = total.ToString();
        }
        else
        {
            shieldRoot.SetActive(false);
        }
    }

    void TrySubscribe()
    {
        if (statusController == null) return;

        statusController.OnStatusesChanged += Refresh;
        usePollingIfNoEvents = false;
    }

    void TryUnsubscribe()
    {
        if (statusController == null) return;

        statusController.OnStatusesChanged -= Refresh;
    }
}

using UnityEngine;
using TMPro;

public class PlayerShieldUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject shieldRoot;      // контейнер (іконка + текст)
    public TMP_Text shieldValueText;   // TMP текст з цифрою

    [Header("Player (optional)")]
    public StatusController statusController; // можна в інспекторі, або auto-find

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

        
        var shield = statusController.Get<ShieldStatus>();

        if (shield != null && shield.AbsorbsLeft > 0)
        {
            shieldRoot.SetActive(true);
            shieldValueText.text = shield.AbsorbsLeft.ToString();
        }
        else
        {
            shieldRoot.SetActive(false);
        }
    }

    // ===== Optional event hookup (якщо у StatusController є OnStatusesChanged) =====
    void TrySubscribe()
    {
        if (statusController == null) return;

        // якщо в тебе є івент OnStatusesChanged — просто розкоментуй 2 рядки нижче
        // statusController.OnStatusesChanged += Refresh;
        // usePollingIfNoEvents = false;
    }

    void TryUnsubscribe()
    {
        if (statusController == null) return;

        // statusController.OnStatusesChanged -= Refresh;
    }
}

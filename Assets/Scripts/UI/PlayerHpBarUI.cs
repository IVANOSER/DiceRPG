using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHpBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth health;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text text;

    private void OnEnable()
    {
        health.OnHpChanged.AddListener(UpdateUI);
        UpdateUI(health.CurrentHp, health.MaxHp);
    }

    private void OnDisable()
    {
        health.OnHpChanged.RemoveListener(UpdateUI);
    }

    private void UpdateUI(int current, int max)
    {
        if (slider != null)
        {
            slider.maxValue = max;
            slider.value = current;
        }
        if (text != null)
            text.text = $"{current}/{max}";
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UltimateButtonUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SkillDiceRuntime dice; // <-- твій runtime на D12
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject root; // що показувати/ховати (може бути цей же GO)

    private void Awake()
    {
        if (root == null) root = gameObject;

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void OnEnable()
    {
        if (dice == null) return;

        dice.OnUltimateReadyChanged += HandleReady;
        dice.OnUltimateChargeChanged += HandleCharge;

        // init
        HandleCharge(dice.CurrentCharge, dice.MaxCharge);
        HandleReady(dice.IsUltimateReady);
        RefreshIcon();
    }

    private void OnDisable()
    {
        if (dice == null) return;

        dice.OnUltimateReadyChanged -= HandleReady;
        dice.OnUltimateChargeChanged -= HandleCharge;
    }

    private void RefreshIcon()
    {
        if (iconImage == null || dice == null) return;
        iconImage.sprite = dice.Ultimate != null ? dice.Ultimate.icon : null;
        iconImage.enabled = (iconImage.sprite != null);
    }

    private void HandleReady(bool ready)
    {
        // ти казав: "коли заряд повний — зʼявляється кнопка"
        if (root != null) root.SetActive(ready);
        if (button != null) button.interactable = ready;

        RefreshIcon();
    }

    private void HandleCharge(int cur, int max)
    {
        // якщо захочеш — тут можна обновляти текст/слайдер типу cur/max
        // зараз нічого не робимо
    }

    private void OnClick()
    {
        dice?.TryUseUltimate();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UltimateButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject root;

    private DiceLoadoutRuntime Loadout => DiceLoadoutRuntime.Instance;

    private void Awake()
    {
        if (root == null)
            root = gameObject;

        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
{
    if (Loadout == null || Loadout.Ultimate == null)
    {
        SetVisible(false);
        return;
    }

    var cfg = UltimateConfigLoader.Get();
    bool isCharged = Loadout.IsUltimateReady(cfg.charge.maxCharge);

    // 1) Якщо НЕ заряджена — кнопки нема
    if (!isCharged)
    {
        SetVisible(false);
        return;
    }

    // 2) Якщо заряджена — кнопка є завжди
    SetVisible(true);

    bool isPlayerTurn = TurnManager.Instance != null && TurnManager.Instance.IsPlayerTurn;
    bool canUse = isPlayerTurn; // заряд уже true

    // іконка
    if (iconImage != null)
    {
        var ultimate = Loadout.Ultimate;
        iconImage.sprite = ultimate.icon;
        iconImage.enabled = ultimate.icon != null;

        // опційно: зробити іконку прозорішою коли не твій хід
        var c = iconImage.color;
        c.a = canUse ? 1f : 0.35f;
        iconImage.color = c;
    }

    // інтерактивність
    if (button != null)
        button.interactable = canUse;
}




    private void SetVisible(bool visible)
    {
        if (root != null && root.activeSelf != visible)
            root.SetActive(visible);
    }

    private void OnClick()
    {
        Loadout?.TryUseUltimate();
    }
}

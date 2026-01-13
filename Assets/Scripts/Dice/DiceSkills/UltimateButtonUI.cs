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
        bool ready = Loadout.IsUltimateReady(cfg.charge.maxCharge);

        SetVisible(ready);

        if (iconImage != null)
        {
            iconImage.sprite = Loadout.Ultimate.icon;
            iconImage.enabled = Loadout.Ultimate.icon != null;
        }

        if (button != null)
            button.interactable = ready;
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

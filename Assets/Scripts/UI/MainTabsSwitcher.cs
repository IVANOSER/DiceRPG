using UnityEngine;

public enum MainTabId
{
    Shop,
    Equip,
    Battle,
    MiniGames,
    Training
}

public class MainTabsSwitcher : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject equipCanvas;
    [SerializeField] private GameObject battleCanvas;

    [Header("Tab Views")]
    [SerializeField] private EquipTabView equipTabView;
    [SerializeField] private BattleTabView battleTabView;

    [SerializeField] private MainTabId startTab = MainTabId.Battle;

    private MainTabId currentTab = MainTabId.Equip;

    private void Start()
    {
        SwitchTo(startTab);
    }
    public void SwitchTo(int tabIndex)
    {
        SwitchTo((MainTabId)tabIndex);
    }

    public void SwitchTo(MainTabId tab)
    {
        if (tab == currentTab) return;

        // hide current
        if (currentTab == MainTabId.Equip) equipTabView?.OnHide();
        if (currentTab == MainTabId.Battle) battleTabView?.OnHide();

        equipCanvas?.SetActive(false);
        battleCanvas?.SetActive(false);

        currentTab = tab;

        // show new
        if (currentTab == MainTabId.Equip)
        {
            equipCanvas?.SetActive(true);
            equipTabView?.OnShow();
        }

        if (currentTab == MainTabId.Battle)
        {
            battleCanvas?.SetActive(true);
            battleTabView?.OnShow();
        }
    }
}

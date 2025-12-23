using UnityEngine;

public class BottomNavController : MonoBehaviour
{
    public enum TabId { Shop, Equip, Battle, Minigames, Training }

    [System.Serializable]
    public class Tab
    {
        public TabId id;
        public TabButtonView view;
        public GameObject contentRoot; // Tab_Shop, Tab_Equip...
    }

    public Tab[] tabs;
    public TabId defaultTab = TabId.Battle;

    TabId _current;

    void Start()
    {
        foreach (var t in tabs)
        {
            var captured = t.id;
            t.view.button.onClick.AddListener(() => Open(captured));
        }

        Open(defaultTab);
    }

    public void Open(TabId id)
    {
        _current = id;

        foreach (var t in tabs)
        {
            bool selected = (t.id == id);

            if (t.contentRoot) t.contentRoot.SetActive(selected);
            if (t.view) t.view.SetSelected(selected);
        }
    }
}

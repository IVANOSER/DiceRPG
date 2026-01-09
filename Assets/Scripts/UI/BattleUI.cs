using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    [Header("Popups")]
    [SerializeField] private GameObject exitPopup;
    [SerializeField] private GameObject victoryPopup;

    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text actionText;

    [SerializeField] private Button rollButton;

    [SerializeField] private Button rerollButton;
    [SerializeField] private TMP_Text rerollCountText;
    [SerializeField] private TMP_Text rerollPriceText;
    [SerializeField] private Image rerollPriceIcon;

    private void Awake()
    {
        Instance = this;

    }

    public void Refresh(int attacksLeft, bool isPlayerTurn)
    {
        
    }

    public void ShowExitPopup(bool show) => exitPopup.SetActive(show);

    public void ShowVictory()
    {
        victoryPopup.SetActive(true);
    }

    public void SetActionInteractable(bool interactable, SkillSO pendingSkill)
    {
        if (actionButton) actionButton.interactable = interactable;

        if (actionText)
        {
            if (pendingSkill == null) actionText.text = "Action";
            else actionText.text = pendingSkill.type == SkillType.Attack ? "Attack" : "Heal";
        }
    }
    public void SetRollInteractable(bool interactable)
    {
        if (rollButton != null)
            rollButton.interactable = interactable;
    }

    public void SetRerollVisible(bool visible)
    {
        if (rerollButton != null)
            rerollButton.gameObject.SetActive(visible);
    }

    public void SetRerollInteractable(bool canReroll, int freeRerollsLeft)
{
    if (rerollButton != null)
        rerollButton.interactable = canReroll;

    
    if (freeRerollsLeft > 0)
    {
        if (rerollCountText != null)
        {
            rerollCountText.gameObject.SetActive(true);
            rerollCountText.text = $"FREE: {freeRerollsLeft}";
        }

        if (rerollPriceText != null)
            rerollPriceText.gameObject.SetActive(false);
            rerollPriceIcon.gameObject.SetActive(false);

        return;
    }

    
    int cost = DiceRerollJsonConfig.Get().rerollGemCost;
    bool hasGems = TurnManager.Instance.HasEnoughGemsForReroll();

    if (rerollCountText != null)
        rerollCountText.gameObject.SetActive(false);

    if (rerollPriceText != null)
    {
        rerollPriceText.gameObject.SetActive(true);
        rerollPriceIcon.gameObject.SetActive(true);
        rerollPriceText.text = $"{cost}";

        rerollPriceText.color = hasGems ? Color.white : Color.red;
    }
}


}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance;

    [Header("Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button endTurnButton;

    [Header("Labels")]
    [SerializeField] private TMP_Text attacksLeftText;
    [SerializeField] private TMP_Text turnText;

    [Header("Popups")]
    [SerializeField] private GameObject exitPopup;
    [SerializeField] private GameObject victoryPopup;

    private void Awake()
    {
        Instance = this;

        attackButton.onClick.AddListener(() => TurnManager.Instance.PlayerAttack());
        endTurnButton.onClick.AddListener(() => TurnManager.Instance.EndPlayerTurn());
    }

    public void Refresh(int attacksLeft, bool isPlayerTurn)
    {
        if (attacksLeftText != null) attacksLeftText.text = $"Attacks: {attacksLeft}";
        if (turnText != null) turnText.text = isPlayerTurn ? "Your turn" : "Enemies turn";

        bool hasTarget = BattleManager.Instance != null && BattleManager.Instance.selectedEnemy != null;

        if (attackButton != null)
            attackButton.interactable = isPlayerTurn && attacksLeft > 0 && hasTarget;

        if (endTurnButton != null)
            endTurnButton.interactable = isPlayerTurn;
    }

    public void ShowExitPopup(bool show) => exitPopup.SetActive(show);

    public void ShowVictory()
    {
        victoryPopup.SetActive(true);
    }
}

using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public bool IsPlayerTurn { get; private set; } = true;
    public bool HasRolledThisTurn { get; private set; } = false;

    [Header("Player refs")]
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;

    public SkillSO PendingSkill { get; private set; }

    [SerializeField] private int maxRerollsPerBattle = 2;
    public int RerollsLeft { get; private set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RerollsLeft = maxRerollsPerBattle;
        BeginPlayerTurn();

    }

    // -------- Roll gating --------
    public bool CanRollDice()
    {
        return IsPlayerTurn && !HasRolledThisTurn;
    }

    public void MarkRolled()
    {
        HasRolledThisTurn = true;
        RefreshRollButton();
    }

    private void ResetRollForNewTurn()
    {
        HasRolledThisTurn = false;
        RefreshRollButton();
    }

    // -------- Pending skill --------
    public void SetPendingSkill(SkillSO skill)
    {
        if (!IsPlayerTurn) return;
        PendingSkill = skill;
        RefreshUI();
        RefreshRerollButton();
    }

    // -------- Action --------
    public void OnActionPressed()
    {
        if (!IsPlayerTurn) return;
        if (PendingSkill == null) return;

        if (PendingSkill.type == SkillType.Attack)
        {
            if (BattleManager.Instance == null || BattleManager.Instance.selectedEnemy == null)
            {
                RefreshUI();
                return;
            }

            int dmgBase = (playerStats != null) ? playerStats.Damage : 1;
            int dmg = dmgBase + PendingSkill.baseValue;
            BattleManager.Instance.AttackSelected(dmg);
        }
        else if (PendingSkill.type == SkillType.Heal)
        {
            int heal = Mathf.Max(0, PendingSkill.baseValue);
            playerHealth.Heal(heal);
        }

        PendingSkill = null;
        RefreshUI();
        RefreshRerollButton();
        EndPlayerTurn();
    }

    public bool CanPressAction()
    {
        if (!IsPlayerTurn) return false;
        if (PendingSkill == null) return false;

        if (PendingSkill.type == SkillType.Attack)
            return BattleManager.Instance != null && BattleManager.Instance.selectedEnemy != null;

        return true;
    }

    public void RefreshActionState()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.SetActionInteractable(CanPressAction(), PendingSkill);
    }

    // -------- Turn flow --------
    public void BeginPlayerTurn()
    {
        IsPlayerTurn = true;
        HasRolledThisTurn = false;
        PendingSkill = null;           // важливо: на новий хід нема pending
        RefreshUI();
        RefreshRerollButton();
    }

    private void EndPlayerTurn()
    {
        if (!IsPlayerTurn) return;

        IsPlayerTurn = false;

        // На ходу ворогів Roll точно має бути вимкнений
        RefreshRollButton();
        RefreshUI();

        StartCoroutine(EnemiesTurn());
    }

    private IEnumerator EnemiesTurn()
    {
        foreach (var enemy in BattleManager.Instance.AliveEnemies)
        {
            if (enemy == null) continue;

            playerHealth.TakeDamage(enemy.AttackDamage);
            yield return new WaitForSeconds(0.4f);

            if (playerHealth.CurrentHp <= 0)
                yield break;
        }

       
        BeginPlayerTurn();
    }

    // -------- UI refresh --------
    private void RefreshUI()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.Refresh(0, IsPlayerTurn);

        RefreshActionState();
        RefreshRollButton();
    }

    private void RefreshRollButton()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.SetRollInteractable(CanRollDice());
    }
    public void ResetAttacksForNewWave()
    {
        BeginPlayerTurn();
    }

    public bool CanReroll()
    {
        // рерол тільки на ходу гравця, тільки якщо вже є результат ролу (PendingSkill),
        // тільки якщо не 0 реролів
        return IsPlayerTurn && PendingSkill != null && RerollsLeft > 0;
    }

    public void ConsumeReroll()
    {
        RerollsLeft = Mathf.Max(0, RerollsLeft - 1);
        RefreshRerollButton();
    }

    private void RefreshRerollButton()
    {
        if (BattleUI.Instance != null)
            BattleUI.Instance.SetRerollVisible(IsPlayerTurn && PendingSkill != null);

        if (BattleUI.Instance != null)
            BattleUI.Instance.SetRerollInteractable(CanReroll(), RerollsLeft);
    }


}

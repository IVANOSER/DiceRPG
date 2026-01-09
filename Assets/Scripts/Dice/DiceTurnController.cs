using UnityEngine;

public class DiceTurnController : MonoBehaviour
{
    public enum SelectedDie
    {
        None,
        SkillD12
    }

    [Header("Player refs")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStats playerStats;

    [Header("Optional: selection highlight (rings / outlines)")]
    [SerializeField] private GameObject skillSelectedFx;

    [Header("State (read-only)")]
    [SerializeField] private SelectedDie selectedDie = SelectedDie.None;

    public SkillSO RolledSkill { get; private set; }

    public bool HasRolledThisAction { get; private set; }
    public bool RerollUsed { get; private set; }

    // ---------- Selection (call from 3D click/tap) ----------
    public void SelectSkillDie()
    {
        selectedDie = SelectedDie.SkillD12;
        RefreshSelectionFx();
    }

    private void RefreshSelectionFx()
    {
        if (skillSelectedFx) skillSelectedFx.SetActive(selectedDie == SelectedDie.SkillD12);
    }

    // ---------- Rolling ----------
    public void RollBoth()
    {
        if (!CanRollOrAct()) return;

        RerollUsed = false;
        HasRolledThisAction = true;

        // default selection after roll (nice UX)
        if (selectedDie == SelectedDie.None)
        {
            selectedDie = SelectedDie.SkillD12;
            RefreshSelectionFx();
        }
    }

    // One button reroll:
    public void RerollSelected()
    {
        if (!CanRollOrAct()) return;
        if (!HasRolledThisAction) return;
        if (RerollUsed) return;

        switch (selectedDie)
        {
            case SelectedDie.SkillD12:
                // Тут має бути твоя логіка ре-ролу SkillD12 (якщо вона в іншому скрипті/анімації — виклич її тут)
                RerollUsed = true;
                return;

            case SelectedDie.None:
            default:
                // no die selected — do nothing (or show UI hint)
                return;
        }
    }

    // ---------- Action ----------
    public void DoAction()
    {
        if (!CanRollOrAct()) return;
        if (!HasRolledThisAction) return;
        if (RolledSkill == null) return;

        // Skill decides target and what we do:
        if (RolledSkill.type == SkillType.Attack)
        {
            if (BattleManager.Instance == null) return;
            if (BattleManager.Instance.selectedEnemy == null) return;

            int baseAtk = (playerStats != null) ? playerStats.Damage : 1;
            int skillBase = RolledSkill.baseValue;

            int totalDamage = baseAtk + skillBase;
            BattleManager.Instance.AttackSelected(totalDamage);
        }
        else if (RolledSkill.type == SkillType.Heal)
        {
            if (playerHealth == null) return;

            int totalHeal = RolledSkill.baseValue;
            playerHealth.Heal(totalHeal);
        }

        // reset state for next action in this turn (if attacksLeft > 0)
        HasRolledThisAction = false;
        RolledSkill = null;
        RerollUsed = false;

        // keep selection (or clear it — up to you)
        // selectedDie = SelectedDie.None;
        // RefreshSelectionFx();
    }

    private bool CanRollOrAct()
    {
        if (TurnManager.Instance == null) return false;
        if (!TurnManager.Instance.IsPlayerTurn) return false;

        return true;
    }
}

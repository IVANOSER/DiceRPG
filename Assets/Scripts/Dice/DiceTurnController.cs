using UnityEngine;

public class DiceTurnController : MonoBehaviour
{
    public enum SelectedDie
    {
        None,
        SkillD12,
        ModD6
    }

    [Header("3D Dice")]
    [SerializeField] private SkillDie3D skillDie3D;
    [SerializeField] private ModDie3D modDie3D;

    [Header("Player refs")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerStats playerStats;

    [Header("Optional: selection highlight (rings / outlines)")]
    [SerializeField] private GameObject skillSelectedFx;
    [SerializeField] private GameObject modSelectedFx;

    [Header("State (read-only)")]
    [SerializeField] private SelectedDie selectedDie = SelectedDie.None;

    public SkillSO RolledSkill { get; private set; }
    public ModifierSO RolledMod { get; private set; }

    public bool HasRolledThisAction { get; private set; }
    public bool RerollUsed { get; private set; }

    // ---------- Selection (call from 3D click/tap) ----------
    public void SelectSkillDie()
    {
        selectedDie = SelectedDie.SkillD12;
        RefreshSelectionFx();
    }

    public void SelectModDie()
    {
        selectedDie = SelectedDie.ModD6;
        RefreshSelectionFx();
    }

    private void RefreshSelectionFx()
    {
        if (skillSelectedFx) skillSelectedFx.SetActive(selectedDie == SelectedDie.SkillD12);
        if (modSelectedFx) modSelectedFx.SetActive(selectedDie == SelectedDie.ModD6);
    }

    // ---------- Rolling ----------
    public void RollBoth()
    {
        if (!CanRollOrAct()) return;

        RerollUsed = false;
        HasRolledThisAction = true;

        RolledSkill = (skillDie3D != null) ? skillDie3D.ThrowRandom() : null;
        RolledMod = (modDie3D != null) ? modDie3D.ThrowRandom() : null;

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
                if (skillDie3D == null) return;
                RolledSkill = skillDie3D.ThrowRandom();
                RerollUsed = true;
                break;

            case SelectedDie.ModD6:
                if (modDie3D == null) return;
                RolledMod = modDie3D.ThrowRandom();
                RerollUsed = true;
                break;

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
            // need selected enemy like before
            if (BattleManager.Instance == null) return;
            if (BattleManager.Instance.selectedEnemy == null) return;

            int baseAtk = (playerStats != null) ? playerStats.Damage : 1;
            int skillBase = RolledSkill.baseValue;

            int mod = 0;
            if (RolledMod != null && !RolledMod.isEmpty)
                mod = RolledMod.attackBonus;

            int totalDamage = baseAtk + skillBase + mod;
            BattleManager.Instance.AttackSelected(totalDamage);
        }
        else if (RolledSkill.type == SkillType.Heal)
        {
            if (playerHealth == null) return;

            int skillBase = RolledSkill.baseValue;

            int mod = 0;
            if (RolledMod != null && !RolledMod.isEmpty)
                mod = RolledMod.healBonus;

            int totalHeal = skillBase + mod;
            playerHealth.Heal(totalHeal);
        }

        // consume "one action/attack" (you have attacksPerTurn = 2)
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.attacksLeft--;

            // refresh UI same as your TurnManager does
            if (BattleUI.Instance != null)
                BattleUI.Instance.Refresh(TurnManager.Instance.attacksLeft, TurnManager.Instance.IsPlayerTurn);
        }

        // reset state for next action in this turn (if attacksLeft > 0)
        HasRolledThisAction = false;
        RolledSkill = null;
        RolledMod = null;
        RerollUsed = false;

        // keep selection (or clear it — up to you)
        // selectedDie = SelectedDie.None;
        // RefreshSelectionFx();
    }

    private bool CanRollOrAct()
    {
        if (TurnManager.Instance == null) return false;
        if (!TurnManager.Instance.IsPlayerTurn) return false;
        if (TurnManager.Instance.attacksLeft <= 0) return false;

        return true;
    }
}

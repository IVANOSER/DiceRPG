using UnityEngine;

public static class SkillResolver
{
    public static bool Apply(SkillSO skill, PlayerStats playerStats, PlayerHealth playerHealth, GameObject target)
    {
        if (skill == null) return false;

        switch (skill.type)
        {
            case SkillType.Attack:
                if (BattleManager.Instance == null || BattleManager.Instance.selectedEnemy == null)
                    return false;

                int dmgBase = (playerStats != null) ? playerStats.Damage : 1;
                int dmg = dmgBase + skill.baseValue;
                BattleManager.Instance.AttackSelected(dmg);
                return true;

            case SkillType.Heal:
                if (playerHealth == null) return false;

                int heal = Mathf.Max(0, skill.baseValue);
                playerHealth.Heal(heal);
                BattleUI.Instance?.Refresh(0, true);

                if (BattleVFXSystem.I != null)
                    BattleVFXSystem.I.SpawnHeal(playerHealth.transform);

                BattleHitFX.PlayHeal(playerHealth.gameObject);
                return true;

            case SkillType.Status:
                if (target == null) return false;
                return ApplyStatus(skill, target);

            default:
                return false;
        }
    }

    private static bool ApplyStatus(SkillSO skill, GameObject target)
    {
        var sc = target.GetComponent<StatusController>();
        if (sc == null) sc = target.AddComponent<StatusController>();

        switch (skill.statusType)
        {
            case StatusType.Stun:
                sc.RefreshOrAddStun(skill.statusDurationTurns);
                return true;

            default:
                return false;
        }
    }
}

using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public bool IsPlayerTurn { get; private set; } = true;

    [SerializeField] private int attacksPerTurn = 2;
   // [SerializeField] private int attackDamage = 5;

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;

    //private int attacksLeft;
    public int attacksLeft;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        attacksLeft = attacksPerTurn;
        BattleUI.Instance.Refresh(attacksLeft, IsPlayerTurn);
    }

    public void PlayerAttack()
    {
        if (!IsPlayerTurn) return;
        if (attacksLeft <= 0) return;
        if (BattleManager.Instance.selectedEnemy == null) return;

        int dmg = playerStats != null ? playerStats.Damage : 1;
        BattleManager.Instance.AttackSelected(dmg);

        attacksLeft--;
        BattleUI.Instance.Refresh(attacksLeft, IsPlayerTurn);
    }

    public void EndPlayerTurn()
    {
        if (!IsPlayerTurn) return;

        IsPlayerTurn = false;
        BattleUI.Instance.Refresh(attacksLeft, IsPlayerTurn);
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

        attacksLeft = attacksPerTurn;
        IsPlayerTurn = true;
        BattleUI.Instance.Refresh(attacksLeft, IsPlayerTurn);
    }

    public void ResetAttacksForNewWave()
    {
        attacksLeft = attacksPerTurn;
        BattleUI.Instance.Refresh(attacksLeft, IsPlayerTurn);
    }
}

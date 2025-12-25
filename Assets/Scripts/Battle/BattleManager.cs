using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public Enemy selectedEnemy;
    private readonly List<Enemy> aliveEnemies = new();

    private void Awake()
    {
        Instance = this;
    }

    public IReadOnlyList<Enemy> AliveEnemies => aliveEnemies;

    public void RegisterEnemy(Enemy e)
    {
        if (e == null) return;
        if (!aliveEnemies.Contains(e))
            aliveEnemies.Add(e);
    }

    public void ClearAliveEnemiesList()
    {
        aliveEnemies.Clear();
        selectedEnemy = null;
    }

    public void SelectEnemy(Enemy enemy)
    {
        if (!TurnManager.Instance.IsPlayerTurn) return;
        if (selectedEnemy == enemy) return;

        // вимкнути на старому
        if (selectedEnemy != null)
        {
            var oldVis = selectedEnemy.GetComponentInChildren<EnemySelectionVisual>(true);
            if (oldVis != null) oldVis.SetSelected(false);
        }

        selectedEnemy = enemy;

        // увімкнути на новому
        if (selectedEnemy != null)
        {
            var newVis = selectedEnemy.GetComponentInChildren<EnemySelectionVisual>(true);
            if (newVis != null) newVis.SetSelected(true);
        }

        BattleUI.Instance.Refresh(TurnManager.Instance.attacksLeft, TurnManager.Instance.IsPlayerTurn);
    }


    public void AttackSelected(int damage)
    {
        if (selectedEnemy == null) return;
        selectedEnemy.TakeDamage(damage);
    }

    public void OnEnemyKilled(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);

        if (selectedEnemy == enemy) selectedEnemy = null;

        if (aliveEnemies.Count == 0)
        {
            StartCoroutine(WorldSphere.Instance.Rotate360AndSpawnNextWave());
        }

        BattleUI.Instance.Refresh(TurnManager.Instance.attacksLeft, TurnManager.Instance.IsPlayerTurn);

    }
}

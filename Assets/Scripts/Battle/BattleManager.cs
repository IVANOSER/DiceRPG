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
        if (selectedEnemy == enemy) return;

        // зняти підсвітку зі старого
        if (selectedEnemy != null)
        {
            var oldHighlight = selectedEnemy.GetComponent<EnemyHighlight>();
            if (oldHighlight != null)
                oldHighlight.SetHighlighted(false);
        }

        selectedEnemy = enemy;

        // увімкнути підсвітку
        var newHighlight = selectedEnemy.GetComponent<EnemyHighlight>();
        if (newHighlight != null)
            newHighlight.SetHighlighted(true);

        Debug.Log("Selected: " + enemy.name);

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

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int attackDamage = 3;
    [SerializeField] private EnemyHealth health;

    public int AttackDamage => attackDamage;

    private void Awake()
    {
        if (health == null) health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        if (health != null) health.OnDied.AddListener(Die);
    }

    private void OnDisable()
    {
        if (health != null) health.OnDied.RemoveListener(Die);
    }

    public void TakeDamage(int dmg)
    {
        health.TakeDamage(dmg);
    }

    private void Die()
    {
        BattleManager.Instance.OnEnemyKilled(this);
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int attackDamage = 3;
    [SerializeField] private EnemyHealth health;

    public int AttackDamage => attackDamage;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDied.AddListener(Die);
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied.RemoveListener(Die);
    }

    public void TakeDamage(int dmg)
    {

        Vector3 hitFrom =
            Camera.main != null
                ? Camera.main.transform.position
                : transform.position + Vector3.back;

        BattleHitFX.PlayHit(
            gameObject,
            hitFrom,
            strength01: 0.6f
        );

        if (BattleVFXSystem.I != null)
            BattleVFXSystem.I.SpawnHitImpact(transform);


        // Реальний дамаг
        health.TakeDamage(dmg);
    }

    private void Die()
    {
        BattleManager.Instance.OnEnemyKilled(this);
        Destroy(gameObject);
    }
}

using UnityEngine;

public class BurnStatus : StatusEffect
{
    private readonly int damagePerTurn;

    public BurnStatus(int turns, int dmgPerTurn) : base(turns)
    {
        damagePerTurn = Mathf.Max(0, dmgPerTurn);
    }

    public override void OnTurnStart(StatusController target)
    {
        if (damagePerTurn <= 0) return;

        var enemy = target.GetComponent<Enemy>();
        if (enemy != null)
            enemy.TakeDamage(damagePerTurn);
    }
}

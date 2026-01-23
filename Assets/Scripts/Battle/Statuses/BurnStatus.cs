using UnityEngine;

public class BurnStatus : StatusEffect
{
    public int DamagePerTurn { get; private set; }

    public BurnStatus(int turns, int dmgPerTurn) : base(turns)
    {
        DamagePerTurn = Mathf.Max(0, dmgPerTurn);
    }

    public void SetDamagePerTurn(int value)
    {
        DamagePerTurn = Mathf.Max(0, value);
    }

    public override void OnTurnStart(StatusController target)
    {
        if (DamagePerTurn <= 0) return;

        var enemy = target.GetComponent<Enemy>();
        if (enemy != null)
            enemy.TakeDamage(DamagePerTurn);
    }
}

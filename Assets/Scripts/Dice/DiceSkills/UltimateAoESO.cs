using UnityEngine;

[CreateAssetMenu(menuName = "Ultimates/AOE Ultimate")]
public class UltimateAoESO : UltimateSO
{
    [Header("Status to ENEMIES")]
    public UltimateStatusKind status = UltimateStatusKind.None;

    public override string PresetKey => "aoe";

    public override void Execute(UltimateContext ctx, UltimateConfig cfg)
    {
        if (ctx.allEnemies == null) return;

        var p = cfg.aoe;

        foreach (var e in ctx.allEnemies)
        {
            if (e == null) continue;

            if (p.damage > 0) e.TakeDamage(p.damage);

            var sc = e.GetComponent<StatusController>();
            if (sc == null) continue;

            if (status == UltimateStatusKind.Burn)
                sc.RefreshOrAddBurn(p.burnTurns, p.burnDamagePerTurn);
            else if (status == UltimateStatusKind.Stun)
                sc.RefreshOrAddStun(p.stunTurns);
        }
    }
}

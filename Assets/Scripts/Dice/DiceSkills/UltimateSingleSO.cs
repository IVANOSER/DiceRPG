using UnityEngine;

[CreateAssetMenu(menuName = "Ultimates/Single Target Ultimate")]
public class UltimateSingleSO : UltimateSO
{
    [Header("Status to ENEMY")]
    public UltimateStatusKind status = UltimateStatusKind.None;

    public override string PresetKey => "single";

    public override void Execute(UltimateContext ctx, UltimateConfig cfg)
    {
        if (ctx.target == null) return;

        var p = cfg.single;

        if (p.damage > 0) ctx.target.TakeDamage(p.damage);

        var sc = ctx.target.GetComponent<StatusController>();
        if (sc == null) return;

        if (status == UltimateStatusKind.Burn)
            sc.RefreshOrAddBurn(p.burnTurns, p.burnDamagePerTurn);
        else if (status == UltimateStatusKind.Stun)
            sc.RefreshOrAddStun(p.stunTurns);
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "Ultimates/Self Ultimate")]
public class UltimateSelfSO : UltimateSO
{
    [Header("Status to SELF")]
    public UltimateSelfStatusKind status = UltimateSelfStatusKind.None;

    public override string PresetKey => "self";

    public override void Execute(UltimateContext ctx, UltimateConfig cfg)
    {
        if (ctx.playerRoot == null) return;

        var p = cfg.self;

        var ph = ctx.playerRoot.GetComponent<PlayerHealth>();
        if (ph != null)
        {
            if (p.healFull) ph.HealFull();
            else if (p.healAmount > 0) ph.Heal(p.healAmount);
        }

        if (status == UltimateSelfStatusKind.Shield)
        {
            var sc = ctx.playerRoot.GetComponent<StatusController>();
            if (sc != null)
            {
                // ✅ НЕ додаємо новий ShieldStatus, а стакаємо в існуючий
                sc.StackOrAddShield(p.shieldAbsorbs);
            }
        }
    }
}

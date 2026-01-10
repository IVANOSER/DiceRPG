using UnityEngine;

public static class BattleHitFX
{
    /// <param name="target">Мінька, яка отримала удар</param>
    /// <param name="hitFromWorld">Звідки прилетів удар (позиція атакуючого)</param>
    /// <param name="strength01">0..1 сила (0.3 light, 1 crit)</param>
    public static void PlayHit(GameObject target, Vector3 hitFromWorld, float strength01 = 0.5f)
    {
        if (target == null) return;

        var wobble = target.GetComponentInChildren<MiniatureWobble>(true);
        if (wobble != null)
        {
            float degrees = Mathf.Lerp(wobble.lightHitDegrees, wobble.heavyHitDegrees, Mathf.Clamp01(strength01));
            wobble.Play(hitFromWorld, degrees);
        }

        var flash = target.GetComponentInChildren<MiniatureHitFlash>(true);
        if (flash != null)
            flash.Play();
    }

    public static void PlayHeal(GameObject target)
    {
        if (target == null) return;

        // Поки що: flash. (Пізніше додамо rune ring + particles)
        var flash = target.GetComponentInChildren<MiniatureHitFlash>(true);
        if (flash != null)
            flash.Play();

        flash.Play(new Color(0.35f, 1f, 0.45f));
    }
}

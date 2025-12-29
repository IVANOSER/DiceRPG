using UnityEngine;

[DefaultExecutionOrder(200)]
public class DicePreviewBinder : MonoBehaviour
{
    [Header("Manual icon painters (required for visuals)")]
    [SerializeField] private D12FaceIcons_Manual d12Icons;
    [SerializeField] private D6FaceIcons_Manual d6Icons;

    [Header("Optional: logic dice (can be null for now)")]
    [SerializeField] private SkillDie3D skillDieD12;
    [SerializeField] private ModDie3D modDieD6;

    [SerializeField] private bool autoWire = true;

    private void Reset()
    {
        AutoWire();
    }

    private void OnEnable()
    {
        if (autoWire) AutoWire();

        DiceLoadoutRuntime.OnChanged += ApplyFromRuntime;

        // На випадок, якщо runtime змінився до підписки — застосуємо на наступний кадр
        Invoke(nameof(ApplyFromRuntime), 0f);
    }

    private void OnDisable()
    {
        DiceLoadoutRuntime.OnChanged -= ApplyFromRuntime;
    }

    private void AutoWire()
    {
        if (d12Icons == null) d12Icons = GetComponentInChildren<D12FaceIcons_Manual>(true);
        if (d6Icons == null) d6Icons = GetComponentInChildren<D6FaceIcons_Manual>(true);

        if (skillDieD12 == null) skillDieD12 = GetComponentInChildren<SkillDie3D>(true);
        if (modDieD6 == null) modDieD6 = GetComponentInChildren<ModDie3D>(true);
    }

    public void ApplyFromRuntime()
    {
        var rt = DiceLoadoutRuntime.Instance;
        if (rt == null) return;

        // Логічні дайси (можуть бути null — не критично)
        if (skillDieD12 != null) skillDieD12.SetFaces(rt.SkillFaces);
        if (modDieD6 != null) modDieD6.SetFaces(rt.ModFaces);

        // Візуал іконок
        if (d12Icons != null) d12Icons.Apply(rt.SkillFaces);
        if (d6Icons != null) d6Icons.Apply(rt.ModFaces);
    }
}

using UnityEngine;

[DefaultExecutionOrder(200)]
public class DicePreviewBinder : MonoBehaviour
{
    [Header("Manual icon painters (required for visuals)")]
    [SerializeField] private D12FaceIcons_Manual d12Icons;

    [Header("Optional: logic dice (can be null for now)")]

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

    }

    public void ApplyFromRuntime()
    {
        var rt = DiceLoadoutRuntime.Instance;
        if (rt == null) return;

       

        // Візуал іконок
        if (d12Icons != null) d12Icons.Apply(rt.SkillFaces);
    }
}

using System.Collections;
using UnityEngine;

public class MiniatureWobble : MonoBehaviour
{
    [Header("Default wobble (degrees)")]
    public float lightHitDegrees = 2f;
    public float heavyHitDegrees = 4f;

    [Header("Timing")]
    public float duration = 0.18f;      // 0.12–0.20
    public int oscillations = 3;        // 2–3

    [Header("Axis weights (tabletop feel)")]
    public float xWeight = 1.0f;        // pitch
    public float zWeight = 1.0f;        // roll
    public float yWeight = 0.0f;        // yaw usually 0 for minis

    private Quaternion _initialLocalRot;
    private Coroutine _routine;

    void Awake()
    {
        _initialLocalRot = transform.localRotation;
    }

    void OnDisable()
    {
        transform.localRotation = _initialLocalRot;
        _routine = null;
    }

    public void PlayLight(Vector3 hitFromWorld) => Play(hitFromWorld, lightHitDegrees);
    public void PlayHeavy(Vector3 hitFromWorld) => Play(hitFromWorld, heavyHitDegrees);

    public void Play(Vector3 hitFromWorld, float degrees)
    {
        if (!isActiveAndEnabled) return;

        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(WobbleRoutine(hitFromWorld, degrees));
    }

    private IEnumerator WobbleRoutine(Vector3 hitFromWorld, float degrees)
    {
        Vector3 localDir = transform.InverseTransformDirection((transform.position - hitFromWorld).normalized);

        float x = Mathf.Clamp(localDir.z, -1f, 1f) * xWeight;
        float z = Mathf.Clamp(-localDir.x, -1f, 1f) * zWeight;
        float y = Mathf.Clamp(localDir.x, -1f, 1f) * yWeight;

        Vector3 axis = new Vector3(x, y, z);
        if (axis.sqrMagnitude < 0.001f) axis = new Vector3(1f, 0f, 1f);
        axis.Normalize();

        float t = 0f;
        while (t < duration)
        {
            float n = t / duration;
            float wave = Mathf.Sin(n * Mathf.PI * oscillations);
            float damp = (1f - n) * (1f - n);

            float angle = degrees * wave * damp;
            transform.localRotation = _initialLocalRot * Quaternion.AngleAxis(angle, axis);

            t += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = _initialLocalRot;
        _routine = null;
    }
}

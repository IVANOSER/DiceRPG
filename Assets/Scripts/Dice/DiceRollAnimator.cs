using System;
using System.Collections;
using UnityEngine;

public class DiceRollAnimator : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Transform diceVisual;

    [Header("Top face Quad icon (Quad Renderer)")]
    [SerializeField] private DiceQuadIconRenderer topFaceQuad;

    [Header("Timing")]
    [SerializeField] private float totalDuration = 1.35f;
    [SerializeField] private float snapDuration = 0.35f;

    [Header("Spin (base)")]
    [SerializeField] private float baseYawSpeed = 2200f;
    [SerializeField] private float pitchSpeed = 1400f;
    [SerializeField] private float rollSpeed = 600f;
    [SerializeField] private float speedChaos = 900f;
    [SerializeField] private float chaosFrequency = 6.5f;

    [Header("Sway (side to side rocking)")]
    [SerializeField] private float swayAngle = 22f;
    [SerializeField] private float swayFrequency = 3.2f;
    [SerializeField] private float swayChaos = 0.35f;

    [Header("Snap to SAME final face every time")]
    [SerializeField] private Vector3 fixedEndEuler = new Vector3(25f, 0f, 0f);

    [Range(0f, 1f)]
    [SerializeField] private float revealIconAt = 0.8f;

    private Coroutine _co;
    private bool _playing;

    public bool IsPlaying => _playing;

    [ContextMenu("Capture End Rotation From Current Visual")]
    public void CaptureEndRotationFromCurrent()
    {
        if (!diceVisual) diceVisual = transform;
        fixedEndEuler = diceVisual.rotation.eulerAngles;
        Debug.Log($"[DiceRollAnimator] Captured fixedEndEuler = {fixedEndEuler}");
    }

    public void Play(SkillSO resultSkill, Action onComplete)
    {
        if (_playing) return;
        _co = StartCoroutine(CoPlay(resultSkill, onComplete));
    }

    private IEnumerator CoPlay(SkillSO skill, Action onComplete)
    {
        _playing = true;

        if (!diceVisual) diceVisual = transform;

        // сховати іконку на старті
        if (topFaceQuad) topFaceQuad.SetSkillIcon(null);

        float chaosTime = Mathf.Max(0.05f, totalDuration - snapDuration);

        float seed = UnityEngine.Random.Range(0f, 999f);
        float phaseA = UnityEngine.Random.Range(0f, 10f);
        float phaseB = UnityEngine.Random.Range(0f, 10f);

        float t = 0f;

        // PHASE 1: spin + sway
        while (t < chaosTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / chaosTime);

            float n = (Mathf.PerlinNoise(seed, Time.time * chaosFrequency) - 0.5f) * 2f; // -1..1
            float speedMul = 1f + (n * (speedChaos / Mathf.Max(1f, baseYawSpeed)));

            float yaw = baseYawSpeed * speedMul * Time.deltaTime;
            float pitch = (pitchSpeed * 0.65f + n * 250f) * Time.deltaTime;
            float roll = (rollSpeed + n * 180f) * Time.deltaTime;

            diceVisual.Rotate(Vector3.up, yaw, Space.World);
            diceVisual.Rotate(Vector3.right, pitch, Space.World);
            diceVisual.Rotate(Vector3.forward, roll, Space.World);

            float sinA = Mathf.Sin((Time.time + phaseA) * swayFrequency);
            float sinB = Mathf.Sin((Time.time + phaseB) * (swayFrequency * 0.73f));

            float c1 = (Mathf.PerlinNoise(seed + 10f, Time.time * (swayFrequency * 1.7f)) - 0.5f) * 2f;
            float c2 = (Mathf.PerlinNoise(seed + 20f, Time.time * (swayFrequency * 1.4f)) - 0.5f) * 2f;

            float swayX = (sinA + c1 * swayChaos) * swayAngle;
            float swayZ = (sinB + c2 * swayChaos) * (swayAngle * 0.85f);

            float envelope = Mathf.Sin(k * Mathf.PI); // 0..1..0
            Quaternion swayRot = Quaternion.Euler(swayX * envelope, 0f, swayZ * envelope);

            diceVisual.rotation = diceVisual.rotation * swayRot;

            yield return null;
        }

        // PHASE 2: smooth stop to fixed rotation
        Quaternion snapFrom = diceVisual.rotation;
        Quaternion snapTo = Quaternion.Euler(fixedEndEuler);

        float s = 0f;
        bool iconShown = false;

        while (s < snapDuration)
        {
            s += Time.deltaTime;
            float k = Mathf.Clamp01(s / snapDuration);

            float eased = 1f - Mathf.Pow(1f - k, 4f);
            diceVisual.rotation = Quaternion.Slerp(snapFrom, snapTo, eased);

            if (!iconShown && k >= revealIconAt)
            {
                iconShown = true;
                if (topFaceQuad) topFaceQuad.SetSkillIcon(skill);
            }

            yield return null;
        }

        diceVisual.rotation = snapTo;
        if (!iconShown && topFaceQuad) topFaceQuad.SetSkillIcon(skill);

        _playing = false;
        onComplete?.Invoke();
    }
}

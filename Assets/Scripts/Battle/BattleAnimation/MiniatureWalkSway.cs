using UnityEngine;

public class MiniatureWalkSway : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isMoving = false;

    [Header("Timing")]
    [SerializeField] private float stepFrequency = 2.2f; // кроки/сек (2.0Ц3.0)

    [Header("Sway (rotation)")]
    [SerializeField] private float rollDegrees = 2.0f;   // хитанн€ вл≥во/вправо (Z)
    [SerializeField] private float pitchDegrees = 1.0f;  // легкий нахил вперед/назад (X)

    [Header("Bob (position)")]
    [SerializeField] private float bobUp = 0.015f;       // п≥дйом по Y (0.01Ц0.03)
    [SerializeField] private float bobSide = 0.010f;     // легкий зсув по X (0.00Ц0.02)

    [Header("Smoothing")]
    [SerializeField] private float blendSpeed = 10f;     // €к швидко вмикаЇтьс€/вимикаЇтьс€

    private Vector3 _baseLocalPos;
    private Quaternion _baseLocalRot;
    private float _blend = 0f;

    void Awake()
    {
        _baseLocalPos = transform.localPosition;
        _baseLocalRot = transform.localRotation;
    }

    void OnDisable()
    {
        transform.localPosition = _baseLocalPos;
        transform.localRotation = _baseLocalRot;
        _blend = 0f;
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    public void SetStepFrequency(float freq)
    {
        stepFrequency = Mathf.Max(0.1f, freq);
    }

    void Update()
    {
        float targetBlend = isMoving ? 1f : 0f;
        _blend = Mathf.MoveTowards(_blend, targetBlend, blendSpeed * Time.deltaTime);

        if (_blend <= 0f)
        {
            // повертаЇмос€ в базу
            transform.localPosition = Vector3.Lerp(transform.localPosition, _baseLocalPos, blendSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, _baseLocalRot, blendSpeed * Time.deltaTime);
            return;
        }

        float t = Time.time * stepFrequency * Mathf.PI * 2f;

        // left-right sway
        float s = Mathf.Sin(t);
        // slight second harmonic for Уstep feelФ
        float s2 = Mathf.Sin(t * 2f + 0.6f);

        float roll = rollDegrees * s;
        float pitch = pitchDegrees * (s2 * 0.5f);

        Quaternion walkRot = Quaternion.Euler(pitch, 0f, -roll);
        Vector3 walkPos = _baseLocalPos
                          + new Vector3(bobSide * s, bobUp * Mathf.Abs(s), 0f);

        // blend in/out
        transform.localRotation = Quaternion.Slerp(_baseLocalRot, _baseLocalRot * walkRot, _blend);
        transform.localPosition = Vector3.Lerp(_baseLocalPos, walkPos, _blend);
    }
}

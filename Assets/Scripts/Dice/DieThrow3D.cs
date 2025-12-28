using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DieThrow3D : MonoBehaviour
{
    [Header("Throw")]
    [SerializeField] private float upwardForce = 4f;
    [SerializeField] private float randomForce = 2f;
    [SerializeField] private float torque = 8f;
    [SerializeField] private float settleTime = 1.0f;

    [Header("Faces (rotation that makes THIS face be UP)")]
    [Tooltip("Size must equal number of faces (6 for d6, 12 for d12). Each rotation should represent the die orientation for that face being up.")]
    [SerializeField] private Quaternion[] faceUpRotations;

    private Rigidbody rb;

    public int FaceCount => faceUpRotations != null ? faceUpRotations.Length : 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ThrowToFace(int faceIndex)
    {
        if (FaceCount == 0) { Debug.LogError($"{name}: faceUpRotations not set"); return; }
        faceIndex = Mathf.Clamp(faceIndex, 0, FaceCount - 1);

        StopAllCoroutines();
        StartCoroutine(ThrowRoutine(faceIndex));
    }

    private IEnumerator ThrowRoutine(int faceIndex)
    {
        // reset motion
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // apply impulse + torque for visuals
        Vector3 dir = new Vector3(
            Random.Range(-randomForce, randomForce),
            upwardForce,
            Random.Range(-randomForce, randomForce)
        );
        rb.AddForce(dir, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * torque, ForceMode.Impulse);

        // wait a bit
        yield return new WaitForSeconds(settleTime);

        // snap to exact face orientation
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.rotation = faceUpRotations[faceIndex];
    }
}

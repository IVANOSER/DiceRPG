using UnityEngine;

public class MeteorFallVFX : MonoBehaviour
{
    public float startHeight = 8f;
    public float fallTime = 0.45f;

    public Transform rock;
    public ParticleSystem impactParticles;

    private Vector3 targetPos;
    private float t;

    private void OnEnable()
    {
        targetPos = transform.position;
        Vector3 startPos = targetPos + Vector3.up * startHeight;

        if (rock != null)
            rock.position = startPos;

        t = 0f;
    }

    private void Update()
    {
        if (rock == null) return;

        t += Time.deltaTime / Mathf.Max(0.01f, fallTime);
        float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f); // easeOutCubic

        rock.position = Vector3.Lerp(targetPos + Vector3.up * startHeight, targetPos, eased);

        if (t >= 1f)
        {
            if (impactParticles != null && !impactParticles.isPlaying)
                impactParticles.Play();

            // можна легкий "squash" або камера-шейк (опціонально)
            enabled = false;
        }
    }
}

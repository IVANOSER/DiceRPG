using UnityEngine;

public class LightningVFX : MonoBehaviour
{
    public LineRenderer line;
    public ParticleSystem impactParticles;

    public float startHeight = 10f;
    public int segments = 10;
    public float jitter = 0.4f;

    private void OnEnable()
    {
        var target = transform.position;
        var start = target + Vector3.up * startHeight;

        if (line != null)
        {
            line.positionCount = segments;

            for (int i = 0; i < segments; i++)
            {
                float a = i / (float)(segments - 1);
                Vector3 p = Vector3.Lerp(start, target, a);

                // джиттер тільки по XZ, щоб лінія “ламалась”
                p += new Vector3(
                    Random.Range(-jitter, jitter),
                    0f,
                    Random.Range(-jitter, jitter)
                ) * (1f - a);

                line.SetPosition(i, p);
            }
        }

        if (impactParticles != null)
            impactParticles.Play();
    }
}

using UnityEngine;

public class MeteorFallVFX : MonoBehaviour
{
    [Header("Refs")]
    public Transform rock;                 // дитина Rock
    public ParticleSystem impactParticles; // дитина ImpactParticles

    [Header("Motion")]
    public float startHeight = 8f;
    public float fallTime = 0.45f;

    [Header("Impact Punch (optional)")]
    public float impactScalePunch = 0.15f;
    public float punchTime = 0.12f;

    [Header("Auto Destroy (optional)")]
    public float autoDestroyAfter = 2.5f;  // щоб префаб сам прибравс€

    private Vector3 targetPos;
    private float t;
    private Vector3 rockBaseScale;
    private bool impacted;

    private void OnEnable()
    {
        // €кщо об'Їкт реюзаЇтьс€/вмикаЇтьс€ повторно Ч все скидаЇмо
        enabled = true;
        impacted = false;

        targetPos = transform.position;

        if (rock != null)
        {
            rock.position = targetPos + Vector3.up * startHeight;
            rockBaseScale = rock.localScale;
        }

        t = 0f;

        if (autoDestroyAfter > 0f)
            Destroy(gameObject, autoDestroyAfter);
    }

    private void Update()
    {
        if (rock == null || impacted) return;

        t += Time.deltaTime / Mathf.Max(0.01f, fallTime);
        float a = Mathf.Clamp01(t);

        // ease-out cubic
        float eased = 1f - Mathf.Pow(1f - a, 3f);

        rock.position = Vector3.Lerp(
            targetPos + Vector3.up * startHeight,
            targetPos,
            eased
        );

        if (a >= 1f)
        {
            Impact();
        }
    }

    private void Impact()
    {
        impacted = true;

        if (impactParticles != null)
            impactParticles.Play();

        if (rock != null && impactScalePunch > 0f)
            StartCoroutine(PunchScale());

        // б≥льше не треба Update()
        enabled = false;
    }

    private System.Collections.IEnumerator PunchScale()
    {
        float half = Mathf.Max(0.01f, punchTime) * 0.5f;
        Vector3 up = rockBaseScale * (1f + impactScalePunch);

        float t1 = 0f;
        while (t1 < 1f)
        {
            t1 += Time.deltaTime / half;
            if (rock != null) rock.localScale = Vector3.Lerp(rockBaseScale, up, t1);
            yield return null;
        }

        float t2 = 0f;
        while (t2 < 1f)
        {
            t2 += Time.deltaTime / half;
            if (rock != null) rock.localScale = Vector3.Lerp(up, rockBaseScale, t2);
            yield return null;
        }
    }
}

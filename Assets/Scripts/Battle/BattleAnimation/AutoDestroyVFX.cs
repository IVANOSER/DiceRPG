using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    [SerializeField] private float extra = 0.2f;

    private void Start()
    {
        var ps = GetComponentInChildren<ParticleSystem>(true);
        if (ps == null)
        {
            Destroy(gameObject);
            return;
        }

        var main = ps.main;
        float time = main.duration + main.startLifetime.constantMax + extra;
        Destroy(gameObject, time);
    }
}

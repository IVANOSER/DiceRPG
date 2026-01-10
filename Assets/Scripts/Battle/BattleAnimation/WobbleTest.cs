using UnityEngine;

public class WobbleTest : MonoBehaviour
{
    [Range(0f, 1f)] public float strength01 = 0.7f;

    [ContextMenu("Test Hit")]
    public void TestHit()
    {
        var wobble = GetComponentInChildren<MiniatureWobble>(true);
        var flash = GetComponentInChildren<MiniatureHitFlash>(true);

        if (wobble != null)
        {
            Vector3 from = Camera.main ? Camera.main.transform.position : (transform.position + Vector3.back);
            float degrees = Mathf.Lerp(wobble.lightHitDegrees, wobble.heavyHitDegrees, strength01);
            wobble.Play(from, degrees);
        }
        else
        {
            Debug.LogWarning("[WobbleTest] MiniatureWobble NOT found in children.");
        }

        if (flash != null)
        {
            flash.Play();
        }
        else
        {
            Debug.LogWarning("[WobbleTest] MiniatureHitFlash NOT found in children.");
        }
    }
}

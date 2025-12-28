using UnityEngine;

public class DicePreviewBinder : MonoBehaviour
{
    [Header("Dice components in this scene")]
    [SerializeField] private SkillDie3D skillDie;
    [SerializeField] private ModDie3D modDie;

    [Header("Face icon renderers (Positions1..N)")]
    [SerializeField] private D12FaceIcons_Positions d12Icons;
    [SerializeField] private D6FaceIcons_Positions d6Icons;

    private void Start()
    {
        ApplyFromRuntime(); // щоб в лоббі одразу підхопилось
    }

    public void ApplyFromRuntime()
    {
        var rt = DiceLoadoutRuntime.Instance;
        if (rt == null)
        {
            Debug.LogError("[DicePreviewBinder] DiceLoadoutRuntime missing in scene (create DiceRuntime GO in Lobby)");
            return;
        }

        if (skillDie != null) skillDie.SetFaces(rt.SkillFaces);
        if (modDie != null) modDie.SetFaces(rt.ModFaces);

        if (d12Icons != null) d12Icons.Apply(rt.SkillFaces);
        if (d6Icons != null) d6Icons.Apply(rt.ModFaces);
    }
}

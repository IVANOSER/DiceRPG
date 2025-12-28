using UnityEngine;

public class DiceLoadoutRuntime : MonoBehaviour
{
    public static DiceLoadoutRuntime Instance { get; private set; }

    [Header("Runtime faces")]
    [SerializeField] private SkillSO[] skillFaces = new SkillSO[12];
    [SerializeField] private ModifierSO[] modFaces = new ModifierSO[6];

    public SkillSO[] SkillFaces => skillFaces;
    public ModifierSO[] ModFaces => modFaces;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (skillFaces == null || skillFaces.Length != 12) skillFaces = new SkillSO[12];
        if (modFaces == null || modFaces.Length != 6) modFaces = new ModifierSO[6];
    }

    // Викликай це після будь-якої зміни екіпу
    public void RebuildSkillFacesFromEquipped(SkillSO[] equipped6)
    {
        if (equipped6 == null) return;

        int idx = 0;
        for (int i = 0; i < 6; i++)
        {
            SkillSO s = (i < equipped6.Length) ? equipped6[i] : null;

            if (idx < 12) skillFaces[idx++] = s;
            if (idx < 12) skillFaces[idx++] = s;
        }

        // safety
        for (; idx < 12; idx++) skillFaces[idx] = null;
    }

    // Один раз задаєш D6 (або міняєш при апгрейді кубика)
    public void SetModFaces(ModifierSO[] faces6)
    {
        if (faces6 == null || faces6.Length != 6) return;
        for (int i = 0; i < 6; i++) modFaces[i] = faces6[i];
    }
}

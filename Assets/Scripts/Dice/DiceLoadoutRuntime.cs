using System;
using UnityEngine;

public class DiceLoadoutRuntime : MonoBehaviour
{
    public static DiceLoadoutRuntime Instance { get; private set; }
    public static event Action OnChanged;

    [SerializeField] private SkillSO[] skillFaces = new SkillSO[12];
    [SerializeField] private ModifierSO[] modFaces = new ModifierSO[6];

    public SkillSO[] SkillFaces => skillFaces;
    public ModifierSO[] ModFaces => modFaces;

    // Авто-створення runtime завжди, без ручних GO
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;

        var go = new GameObject("DiceRuntime");
        go.AddComponent<DiceLoadoutRuntime>();
        DontDestroyOnLoad(go);
    }

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

    public void RebuildSkillFacesFromEquipped(SkillSO[] equipped6)
    {
        int idx = 0;

        for (int i = 0; i < 6; i++)
        {
            SkillSO s = (equipped6 != null && i < equipped6.Length) ? equipped6[i] : null;

            if (idx < 12) skillFaces[idx++] = s;
            if (idx < 12) skillFaces[idx++] = s;
        }

        for (; idx < 12; idx++) skillFaces[idx] = null;

        OnChanged?.Invoke();
    }

    public void SetModFaces(ModifierSO[] faces6)
    {
        if (modFaces == null || modFaces.Length != 6) modFaces = new ModifierSO[6];

        if (faces6 == null || faces6.Length != 6)
        {
            for (int i = 0; i < 6; i++) modFaces[i] = null;
            OnChanged?.Invoke();
            return;
        }

        for (int i = 0; i < 6; i++)
            modFaces[i] = faces6[i];

        OnChanged?.Invoke();
    }
}

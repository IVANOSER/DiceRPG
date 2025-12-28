using UnityEngine;

[RequireComponent(typeof(DieThrow3D))]
public class SkillDie3D : MonoBehaviour
{
    [SerializeField] private SkillSO[] faces = new SkillSO[12];

    private DieThrow3D throw3D;

    public SkillSO[] Faces => faces;

    // якщо треба знати останній індекс
    public int LastIndex { get; private set; } = -1;

    private void Awake()
    {
        throw3D = GetComponent<DieThrow3D>();

        if (faces == null || faces.Length != 12)
            faces = new SkillSO[12];
    }

    public void SetFaces(SkillSO[] src)
    {
        if (src == null || src.Length != 12)
        {
            Debug.LogError($"[SkillDie3D] SetFaces expects SkillSO[12], got {(src == null ? "null" : src.Length.ToString())}");
            return;
        }

        if (faces == null || faces.Length != 12)
            faces = new SkillSO[12];

        for (int i = 0; i < 12; i++)
            faces[i] = src[i];
    }

    public SkillSO GetFace(int index)
    {
        if (faces == null || faces.Length != 12) return null;
        if (index < 0 || index >= 12) return null;
        return faces[index];
    }

    /// <summary>
    /// Повертає SkillSO який "випав". Кубик візуально кидається (без snap ок).
    /// </summary>
    public SkillSO ThrowRandom()
    {
        int idx = Random.Range(0, 12);
        LastIndex = idx;

        if (throw3D != null)
            throw3D.ThrowToFace(idx);

        return faces != null && faces.Length == 12 ? faces[idx] : null;
    }
}

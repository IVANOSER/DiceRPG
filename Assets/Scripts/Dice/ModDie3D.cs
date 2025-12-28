using UnityEngine;

[RequireComponent(typeof(DieThrow3D))]
public class ModDie3D : MonoBehaviour
{
    [SerializeField] private ModifierSO[] faces = new ModifierSO[6];

    private DieThrow3D throw3D;

    public ModifierSO[] Faces => faces;

    public int LastIndex { get; private set; } = -1;

    private void Awake()
    {
        throw3D = GetComponent<DieThrow3D>();

        if (faces == null || faces.Length != 6)
            faces = new ModifierSO[6];
    }

    public void SetFaces(ModifierSO[] src)
    {
        if (src == null || src.Length != 6)
        {
            Debug.LogError($"[ModDie3D] SetFaces expects ModifierSO[6], got {(src == null ? "null" : src.Length.ToString())}");
            return;
        }

        if (faces == null || faces.Length != 6)
            faces = new ModifierSO[6];

        for (int i = 0; i < 6; i++)
            faces[i] = src[i];
    }

    public ModifierSO GetFace(int index)
    {
        if (faces == null || faces.Length != 6) return null;
        if (index < 0 || index >= 6) return null;
        return faces[index];
    }

    /// <summary>
    /// Повертає ModifierSO який "випав". Може бути null/empty.
    /// </summary>
    public ModifierSO ThrowRandom()
    {
        int idx = Random.Range(0, 6);
        LastIndex = idx;

        if (throw3D != null)
            throw3D.ThrowToFace(idx);

        return faces != null && faces.Length == 6 ? faces[idx] : null;
    }
}

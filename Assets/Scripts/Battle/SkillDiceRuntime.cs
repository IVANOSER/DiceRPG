using System.Collections.Generic;
using UnityEngine;

public class SkillDiceRuntime : MonoBehaviour
{
    [Header("Runtime faces (12 total)")]
    [SerializeField] private List<SkillSO> faces = new(12);

    public SkillSO LastRoll { get; private set; }

    public void RebuildFromEquippedSkills(List<SkillSO> equippedSkills)
    {
        faces.Clear();

        // кожен скіл ×2, щоб з 6 слотів стало 12 граней
        foreach (var s in equippedSkills)
        {
            if (s == null) continue;
            faces.Add(s);
            faces.Add(s);
        }

        // safety: якщо шмоток менше 6 — добиваємо пустими/дефолтними (або залишаємо як є)
        // Можеш тут додати "EmptySkill" якщо захочеш.
    }

    public SkillSO Roll()
    {
        if (faces.Count == 0)
        {
            Debug.LogWarning("SkillDice faces are empty. RebuildFromEquippedSkills was not called?");
            LastRoll = null;
            return null;
        }

        LastRoll = faces[Random.Range(0, faces.Count)];
        return LastRoll;
    }
}

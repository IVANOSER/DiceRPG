using UnityEngine;

public enum SkillType { Attack, Heal }

[CreateAssetMenu(menuName = "Battle/Dice/Skill")]
public class SkillSO : ScriptableObject
{
    public SkillType type;
    public int baseValue = 1; // база для атаки/хілу
    public Sprite icon;       // для UI (необов'язково)
}

using UnityEngine;

public enum SkillType { Attack, Heal, Status }
public enum StatusType { None, Stun }

[CreateAssetMenu(menuName = "Battle/Dice/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("UI")]
    public string displayName = "Skill"; 

    [Header("Core")]
    public SkillType type;
    public int baseValue = 1;      // дамаг/хіл (або “сила”, якщо захочеш)
    public Sprite icon;

    [Header("Status (only if type = Status)")]
    public StatusType statusType = StatusType.None;
    [Min(1)] public int statusDurationTurns = 1;

    [Header("Drop chance")]
    [Min(0)] public int dropWeight = 1;

    [Header("Targeting")]
    public bool requiresTarget = false;

    
}

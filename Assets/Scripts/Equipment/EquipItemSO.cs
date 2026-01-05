using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Equipment/Equip Item")]
public class EquipItemSO : ScriptableObject
{
    [Header("Setup")]
    public ItemQuality quality = ItemQuality.good;
    public EquipmentSlot slot = EquipmentSlot.Belt;

    [Tooltip("Only this is typed manually. Example: 1 -> 001")]
    [Min(1)]
    public int setNumber = 1;

    [Header("Auto-generated ID")]
    [SerializeField]
    private string id;
    public string Id => id;

    [Header("UI")]
    public string displayName;
    public Sprite icon;

    [Header("Stats (SOURCE OF TRUTH)")]
    public StatModifier[] modifiers;

    [Header("Dice Skill")]
    public SkillSO skill;

    [Header("Extra Skills (shown on card)")]
    public List<SkillSO> extraSkills = new();

    [Header("Synty Mesh Swap")]
    public List<MeshReplace> meshReplaces = new();

    [Serializable]
    public class MeshReplace
    {
        public BodyPartSlot target;
        public Mesh mesh;
        public Material materialOverride;
    }

    // =========================
    // Item Card helpers
    // =========================

    /// <summary>
    /// Text for Item Card stats block (e.g. "+30 HP\n+10 DMG")
    /// </summary>
    public string GetStatsTextForCard()
    {
        if (modifiers == null || modifiers.Length == 0)
            return "No stat bonuses";

        List<string> lines = new();

        foreach (var m in modifiers)
        {
            string statName = FormatStatName(m.type);
            int value = m.value;

            string sign = value >= 0 ? "+" : "";
            lines.Add($"{sign}{value} {statName}");
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Text for Item Card skills block
    /// </summary>
    public string GetSkillsTextForCard()
    {
        List<string> lines = new();

        if (skill != null)
            lines.Add("• " + skill.name);

        foreach (var s in extraSkills)
        {
            if (s != null)
                lines.Add("• " + s.name);
        }

        return lines.Count == 0
            ? "No extra skills"
            : string.Join("\n", lines);
    }

    private string FormatStatName(StatType type)
    {
        // Косметика для UI
        return type switch
        {
            StatType.HP => "HP",
            StatType.Damage => "DMG",
            _ => type.ToString()
        };
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (setNumber < 1)
            setNumber = 1;

        id = BuildId(quality, slot, setNumber);

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = id;
    }
#endif

    public static string BuildId(ItemQuality q, EquipmentSlot s, int setNum)
    {
        return $"{q}.{SlotToCode(s)}.{setNum:000}";
    }

    private static string SlotToCode(EquipmentSlot s)
    {
        return s switch
        {
            EquipmentSlot.Belt => "bl",
            EquipmentSlot.Legs => "lg",
            EquipmentSlot.RightHand => "rhd",
            EquipmentSlot.LeftHand => "lhd",
            EquipmentSlot.Helmet => "hd",
            EquipmentSlot.Chest => "ch",
            _ => "bl"
        };
    }
}

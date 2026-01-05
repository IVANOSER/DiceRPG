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
    [SerializeField, Tooltip("Auto: quality.type.set (e.g. bttr.bl.001)")]
    private string id;
    public string Id => id;

    [Header("UI")]
    [Tooltip("Name shown in UI. Set manually. Will NOT be overwritten by id.")]
    public string displayName;

    public Sprite icon;

    // ✅ ДОДАВ (опційно): якщо хочеш, щоб ТІЛЬКИ один раз підхопило id, а далі не чіпало
    [Tooltip("If enabled, displayName will be filled from id ONLY when displayName is empty.")]
    public bool autoFillDisplayNameIfEmpty = true;

    [Header("Stats (source of truth)")]
    public StatModifier[] modifiers;

    [Header("Dice Skill")]
    public SkillSO skill;

    [Header("Extra Skills (shown on card)")]
    public List<SkillSO> extraSkills = new();

    [Header("Synty Mesh Swap (body parts)")]
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

    public string GetStatsTextForCard()
    {
        if (modifiers == null || modifiers.Length == 0)
            return "No stat bonuses";

        List<string> lines = new(modifiers.Length);

        foreach (var m in modifiers)
        {
            // StatModifier: type + value (як у твоєму файлі)
            string statName = FormatStatName(m.type);
            int v = m.value;
            string sign = v >= 0 ? "+" : "";
            lines.Add($"{sign}{v} {statName}");
        }

        return string.Join("\n", lines);
    }

    public string GetSkillsTextForCard()
    {
        List<string> lines = new();

        if (skill != null)
            lines.Add("• " + skill.name);

        if (extraSkills != null)
        {
            foreach (var s in extraSkills)
                if (s != null) lines.Add("• " + s.name);
        }

        return lines.Count == 0 ? "No extra skills" : string.Join("\n", lines);
    }

    private static string FormatStatName(StatType type)
    {
        // косметика під твій стиль: Damage -> DMG
        return type switch
        {
            StatType.Damage => "DMG",
            StatType.HP => "HP",
            _ => type.ToString()
        };
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (setNumber < 1) setNumber = 1;

        id = BuildId(quality, slot, setNumber);

        // ✅ ФІКС: displayName більше НЕ перезаписується примусово.
        // Тільки якщо autoFill... увімкнений і поле реально пусте.
        if (autoFillDisplayNameIfEmpty && string.IsNullOrWhiteSpace(displayName))
            displayName = id;
    }
#endif

    public static string BuildId(ItemQuality q, EquipmentSlot s, int setNum)
    {
        return $"{q}.{SlotToTypeCode(s)}.{setNum:000}";
    }

    private static string SlotToTypeCode(EquipmentSlot s)
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

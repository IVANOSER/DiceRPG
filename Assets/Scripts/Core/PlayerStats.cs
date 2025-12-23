using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int baseHP = 10;
    public int baseDamage = 1;

    public int HP { get; private set; }
    public int Damage { get; private set; }

    public void Recalculate(PlayerLoadoutSO loadout)
    {
        int hp = baseHP;
        int dmg = baseDamage;

        Apply(loadout.leftHand, ref hp, ref dmg);
        Apply(loadout.rightHand, ref hp, ref dmg);
        Apply(loadout.belt, ref hp, ref dmg);
        Apply(loadout.helmet, ref hp, ref dmg);
        Apply(loadout.chest, ref hp, ref dmg);
        Apply(loadout.legs, ref hp, ref dmg);

        HP = hp;
        Damage = dmg;
    }

    static void Apply(EquipItemSO item, ref int hp, ref int dmg)
    {
        if (item == null || item.modifiers == null) return;
        foreach (var m in item.modifiers)
        {
            if (m.type == StatType.HP) hp += m.value;
            else if (m.type == StatType.Damage) dmg += m.value;
        }
    }
}

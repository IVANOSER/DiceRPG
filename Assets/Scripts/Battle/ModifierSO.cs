using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Dice/Modifier")]
public class ModifierSO : ScriptableObject
{
    public int attackBonus;
    public int healBonus;
    public bool isEmpty;      // якщо це "порожня грань"
    public Sprite icon;       // для UI (необов'язково)
}

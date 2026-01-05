using UnityEngine;

[CreateAssetMenu(menuName = "Game/Drop/Item Drop Config")]
public class ItemDropConfigSO : ScriptableObject
{
    public EquipItemSO[] possibleItems;

    public EquipItemSO GetRandom()
    {
        if (possibleItems == null || possibleItems.Length == 0)
            return null;

        int i = Random.Range(0, possibleItems.Length);
        return possibleItems[i];
    }
}

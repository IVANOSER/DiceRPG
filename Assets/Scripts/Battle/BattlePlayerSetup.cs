using UnityEngine;

public class BattlePlayerSetup : MonoBehaviour
{
    [SerializeField] private PlayerLoadoutSO loadout;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        playerStats.Recalculate(loadout);
        playerHealth.ApplyMaxHpFromStats(playerStats.HP);
    }
}

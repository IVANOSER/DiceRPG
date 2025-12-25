using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSphere : MonoBehaviour
{
    public static WorldSphere Instance;

    [Header("Spawn points on sphere (5 normal + optional boss)")]
    [SerializeField] private List<Transform> enemySpawnPoints = new(); // 5 точок
    [SerializeField] private Transform bossSpawnPoint; // optional

    [Header("Refs")]
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Transform enemiesParent;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeedDegPerSec = 120f;

    [Header("Spawn Rotation Fix")]
    [SerializeField] private Vector3 enemyRotationOffsetEuler = new Vector3(0f, 180f, 0f);

    public enum RotationAxis { X, Y, Z }

    [Header("Rotation Axis")]
    [SerializeField] private RotationAxis rotationAxis = RotationAxis.Y;

    [Header("Next wave spawn timing")]
    [SerializeField] private float spawnAtDegrees = 180f;

    [Header("Rotation Direction")]
    [SerializeField] private bool invertRotationDirection = false;

    [Header("Player refs (for stats refresh)")]
    [SerializeField] private PlayerLoadoutSO loadout;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnCurrentWave();
    }

    public void SpawnCurrentWave()
    {
        var wave = waveManager.CurrentWave;
        if (wave == null) return;

        // чисто підстрахуємось (якщо після тестів щось залишилось)
        BattleManager.Instance.ClearAliveEnemiesList();

        int count = Mathf.Min(wave.enemies.Count, enemySpawnPoints.Count);

        for (int i = 0; i < count; i++)
        {
            var prefab = wave.enemies[i];
            var sp = enemySpawnPoints[i];

            Quaternion rot = sp.rotation * Quaternion.Euler(enemyRotationOffsetEuler);
            var go = Instantiate(prefab, sp.position, rot, enemiesParent);

            var enemy = go.GetComponent<Enemy>();
            if (enemy != null)
                BattleManager.Instance.RegisterEnemy(enemy);
        }

        // (опційно) бос — якщо в хвилі є 6-й елемент і є bossSpawnPoint
        if (bossSpawnPoint != null && wave.enemies.Count > enemySpawnPoints.Count)
        {
            var bossPrefab = wave.enemies[enemySpawnPoints.Count];
            var go = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation, enemiesParent);

            var enemy = go.GetComponent<Enemy>();
            if (enemy != null)
                BattleManager.Instance.RegisterEnemy(enemy);
        }
    }

    public IEnumerator Rotate360AndSpawnNextWave()
    {
        float rotated = 0f;
        bool spawned = false;

        while (rotated < 360f)
        {
            float step = rotateSpeedDegPerSec * Time.deltaTime;
            if (invertRotationDirection) step = -step;

            transform.Rotate(GetRotationAxis(), step, Space.World);
            rotated += Mathf.Abs(step);

            if (!spawned && rotated >= spawnAtDegrees)
            {
                SpawnNextWaveMidRotation();
                spawned = true;
            }

            yield return null;
        }
    }


    private Vector3 GetRotationAxis()
    {
        return rotationAxis switch
        {
            RotationAxis.X => Vector3.right,
            RotationAxis.Y => Vector3.up,
            RotationAxis.Z => Vector3.forward,
            _ => Vector3.up
        };
    }

    private void SpawnNextWaveMidRotation()
    {
        if (!waveManager.HasNextWave)
        {
            BattleUI.Instance.ShowVictory();
            return;
        }

        waveManager.NextWave();
        SpawnCurrentWave();

        playerStats.Recalculate(loadout);
        playerHealth.ApplyMaxHpFromStats(playerStats.HP, healToFull: false); // true якщо хочеш фулл хіл між хвилями
        TurnManager.Instance.ResetAttacksForNewWave();
        TurnManager.Instance.ResetAttacksForNewWave();
    }
}

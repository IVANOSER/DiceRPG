using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    private int currentWaveIndex = 0;

    public int CurrentWaveIndex => currentWaveIndex;
    public int WavesCount => levelConfig != null ? levelConfig.waves.Count : 0;

    public bool HasNextWave => levelConfig != null && currentWaveIndex + 1 < levelConfig.waves.Count;

    public WaveConfig CurrentWave
    {
        get
        {
            if (levelConfig == null || levelConfig.waves.Count == 0) return null;
            return levelConfig.waves[currentWaveIndex];
        }
    }

    public void NextWave()
    {
        if (!HasNextWave) return;
        currentWaveIndex++;
    }
}

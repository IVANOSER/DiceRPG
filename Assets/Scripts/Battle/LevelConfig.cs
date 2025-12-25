using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Level")]
public class LevelConfig : ScriptableObject
{
    public List<WaveConfig> waves;
}

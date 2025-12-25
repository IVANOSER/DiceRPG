using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Wave")]
public class WaveConfig : ScriptableObject
{
    public List<GameObject> enemies;
}

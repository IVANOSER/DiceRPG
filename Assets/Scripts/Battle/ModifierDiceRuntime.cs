using System.Collections.Generic;
using UnityEngine;

public class ModifierDiceRuntime : MonoBehaviour
{
    [SerializeField] private List<ModifierSO> faces = new(6);

    public ModifierSO LastRoll { get; private set; }

    public ModifierSO Roll()
    {
        if (faces == null || faces.Count == 0)
        {
            Debug.LogWarning("ModifierDice faces are empty.");
            LastRoll = null;
            return null;
        }

        LastRoll = faces[Random.Range(0, faces.Count)];
        return LastRoll;
    }
}

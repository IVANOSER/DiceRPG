using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    public UnityEvent<int, int> OnHpChanged; // current, max
    public UnityEvent OnDied;

    public void ApplyMaxHpFromStats(int maxHp, bool healToFull = true)
    {
        MaxHp = Mathf.Max(1, maxHp);

        if (healToFull) CurrentHp = MaxHp;
        else CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    public void TakeDamage(int dmg)
    {
        dmg = Mathf.Max(0, dmg);
        CurrentHp = Mathf.Max(0, CurrentHp - dmg);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);

        if (CurrentHp <= 0)
            OnDied?.Invoke();
    }
}

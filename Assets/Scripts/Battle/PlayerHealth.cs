using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    public UnityEvent<int, int> OnHpChanged; // current, max
    public UnityEvent OnDied;

    /// <summary>
    /// Викликається при старті бою / зміні статів (BattlePlayerSetup)
    /// </summary>
    public void ApplyMaxHpFromStats(int maxHp, bool healToFull = true)
    {
        MaxHp = Mathf.Max(1, maxHp);

        if (healToFull)
            CurrentHp = MaxHp;
        else
            CurrentHp = Mathf.Clamp(CurrentHp, 0, MaxHp);

        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    /// <summary>
    /// Отримання урону (з урахуванням Shield статусу)
    /// </summary>
    public void TakeDamage(int dmg)
{
    var statuses = GetComponent<StatusController>();
    if (statuses != null && statuses.TryUseShieldAbsorb())
        return;

    dmg = Mathf.Max(0, dmg);
    if (dmg <= 0) return;

    
    var wobble = GetComponentInChildren<MiniatureWobble>();
    if (wobble != null)
    {
        Vector3 hitFrom =
            Camera.main != null
                ? Camera.main.transform.position
                : transform.position + Vector3.back;

        wobble.PlayLight(hitFrom); // або PlayHeavy(hitFrom)
    }

    CurrentHp = Mathf.Max(0, CurrentHp - dmg);
    OnHpChanged?.Invoke(CurrentHp, MaxHp);

    if (CurrentHp <= 0)
        OnDied?.Invoke();
}


    /// <summary>
    /// Лікування
    /// </summary>
    public void Heal(int amount)
    {
        amount = Mathf.Max(0, amount);
        if (amount == 0) return;

        int before = CurrentHp;
        CurrentHp = Mathf.Min(CurrentHp + amount, MaxHp);

        if (CurrentHp != before)
            OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    /// <summary>
    /// Повне лікування (для self-ульт)
    /// </summary>
    public void HealFull()
    {
        CurrentHp = MaxHp;
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }
}

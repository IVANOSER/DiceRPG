using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 20;

    public int MaxHp { get; private set; }
    public int CurrentHp { get; private set; }

    public UnityEvent<int, int> OnHpChanged;
    public UnityEvent OnDied;

    private void Awake()
    {
        MaxHp = Mathf.Max(1, maxHp);
        CurrentHp = MaxHp;
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

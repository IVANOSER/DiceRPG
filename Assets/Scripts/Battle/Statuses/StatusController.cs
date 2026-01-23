using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    private readonly List<StatusEffect> _effects = new();

    public event Action OnStatusesChanged;

    public void Add(StatusEffect effect)
    {
        if (effect == null) return;

        _effects.Add(effect);
        effect.OnApply(this);
        OnStatusesChanged?.Invoke();
    }

    public bool Has<T>() where T : StatusEffect
    {
        for (int i = 0; i < _effects.Count; i++)
            if (_effects[i] is T) return true;
        return false;
    }

    public T Get<T>() where T : StatusEffect
    {
        for (int i = 0; i < _effects.Count; i++)
            if (_effects[i] is T t) return t;
        return null;
    }

    // ✅ НОВЕ: отримати всі статуси певного типу (щоб UI міг сумувати)
    public List<T> GetAll<T>() where T : StatusEffect
    {
        var list = new List<T>();
        for (int i = 0; i < _effects.Count; i++)
            if (_effects[i] is T t) list.Add(t);
        return list;
    }

    public void TurnStart()
    {
        if (_effects.Count == 0) return;

        var snapshot = _effects.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
        {
            var e = snapshot[i];
            if (e == null) continue;
            e.OnTurnStart(this);
        }

        OnStatusesChanged?.Invoke();
    }

    public void TurnEnd()
    {
        if (_effects.Count == 0) return;

        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            var e = _effects[i];
            if (e == null)
            {
                _effects.RemoveAt(i);
                continue;
            }

            e.OnTurnEnd(this);

            if (e.IsExpired)
                _effects.RemoveAt(i);
        }

        OnStatusesChanged?.Invoke();
    }

    // ---------------- SHIELD (STACKING) ----------------
    public void StackOrAddShield(int absorbs)
    {
        absorbs = Mathf.Max(0, absorbs);
        if (absorbs <= 0) return;

        var shield = Get<ShieldStatus>();
        if (shield != null)
        {
            shield.AddAbsorbs(absorbs);
            OnStatusesChanged?.Invoke();
            return;
        }

        Add(new ShieldStatus(absorbs));
    }

    // Викликається з PlayerHealth.TakeDamage()
    public bool TryUseShieldAbsorb()
    {
        var shield = Get<ShieldStatus>();
        if (shield == null) return false;

        bool absorbed = shield.TryAbsorbHit();

        if (shield.IsExpired)
            _effects.Remove(shield);

        if (absorbed)
            OnStatusesChanged?.Invoke();

        return absorbed;
    }

    // (опційно) лишаємо для сумісності
    public void RefreshOrAddBurn(int turns, int dmgPerTurn)
    {
        // якщо ти вже робив стакання burn/stun — можеш лишити свою реалізацію
        var burn = Get<BurnStatus>();
        if (burn != null)
        {
            burn.AddTurns(Mathf.Max(1, turns));
            burn.SetDamagePerTurn(Mathf.Max(burn.DamagePerTurn, dmgPerTurn));
            OnStatusesChanged?.Invoke();
            return;
        }

        Add(new BurnStatus(Mathf.Max(1, turns), Mathf.Max(0, dmgPerTurn)));
    }

    public void RefreshOrAddStun(int turns)
    {
        var stun = Get<StunStatus>();
        if (stun != null)
        {
            stun.AddTurns(Mathf.Max(1, turns));
            OnStatusesChanged?.Invoke();
            return;
        }

        Add(new StunStatus(Mathf.Max(1, turns)));
    }
}

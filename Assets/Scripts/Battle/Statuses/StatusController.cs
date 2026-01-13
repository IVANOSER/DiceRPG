using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    private readonly List<StatusEffect> _effects = new();

    public event Action OnStatusesChanged;

    public void Add(StatusEffect effect)
    {
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

    public void TurnStart()
    {
        for (int i = 0; i < _effects.Count; i++)
            _effects[i].OnTurnStart(this);
    }

    public void TurnEnd()
    {
        for (int i = _effects.Count - 1; i >= 0; i--)
        {
            _effects[i].OnTurnEnd(this);
            if (_effects[i].IsExpired)
                _effects.RemoveAt(i);
        }

        OnStatusesChanged?.Invoke();
    }

    // --- STUN (як у тебе) ---
    public void RefreshOrAddStun(int turns)
    {
        for (int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i] is StunStatus stun)
            {
                stun.RefreshToAtLeast(turns);
                OnStatusesChanged?.Invoke();
                return;
            }
        }
        Add(new StunStatus(turns));
    }

    // --- BURN (нове) ---
    public void RefreshOrAddBurn(int turns, int dmgPerTurn)
    {
        for (int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i] is BurnStatus burn)
            {
                burn.RefreshToAtLeast(turns);
                OnStatusesChanged?.Invoke();
                return;
            }
        }
        Add(new BurnStatus(turns, dmgPerTurn));
    }

    // --- SHIELD (нове) ---
    public bool TryUseShieldAbsorb()
    {
        for (int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i] is ShieldStatus shield)
            {
                bool absorbed = shield.TryAbsorbHit();
                if (absorbed) OnStatusesChanged?.Invoke();
                return absorbed;
            }
        }
        return false;
    }
}

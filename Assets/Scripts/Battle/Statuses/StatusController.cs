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

    /// <summary>
    /// Викликається НА ПОЧАТКУ ходу юніта.
    /// Важливо: ітеруємось по snapshot, щоб статуси не "з'їдали" один одного.
    /// </summary>
    public void TurnStart()
    {
        if (_effects.Count == 0) return;

        // snapshot: якщо під час OnTurnStart щось додасться/зміниться — цикл не зламається
        var snapshot = _effects.ToArray();
        for (int i = 0; i < snapshot.Length; i++)
        {
            var e = snapshot[i];
            if (e == null) continue;
            e.OnTurnStart(this);
        }

        OnStatusesChanged?.Invoke();
    }

    /// <summary>
    /// Викликається В КІНЦІ ходу юніта.
    /// Тут декрементимо/очищаємо.
    /// </summary>
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

    // ---------------- STUN ----------------
    public void RefreshOrAddStun(int turns)
    {
        turns = Mathf.Max(1, turns);

        var stun = Get<StunStatus>();
        if (stun != null)
        {
            stun.RefreshToAtLeast(turns);
            OnStatusesChanged?.Invoke();
            return;
        }

        Add(new StunStatus(turns));
    }

    // ---------------- BURN ----------------
    public void RefreshOrAddBurn(int turns, int dmgPerTurn)
    {
        turns = Mathf.Max(1, turns);
        dmgPerTurn = Mathf.Max(0, dmgPerTurn);

        var burn = Get<BurnStatus>();
        if (burn != null)
        {
            burn.RefreshToAtLeast(turns);

            // Якщо в твоєму BurnStatus немає цих методів — скажи, я піджену під твій клас.
            // Але краще мати, бо інакше dmgPerTurn не оновлюється ніколи.
            burn.SetDamagePerTurn(dmgPerTurn);

            OnStatusesChanged?.Invoke();
            return;
        }

        Add(new BurnStatus(turns, dmgPerTurn));
    }

    // ---------------- SHIELD ----------------
    public bool TryUseShieldAbsorb()
    {
        var shield = Get<ShieldStatus>();
        if (shield == null) return false;

        bool absorbed = shield.TryAbsorbHit();

        // якщо стаки скінчились — прибираємо одразу (щоб UI не брехав до TurnEnd)
        if (shield.IsExpired)
            _effects.Remove(shield);

        if (absorbed)
            OnStatusesChanged?.Invoke();

        return absorbed;
    }
}

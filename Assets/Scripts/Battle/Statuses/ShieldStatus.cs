public class ShieldStatus : StatusEffect
{
    public int AbsorbsLeft { get; private set; }

    public ShieldStatus(int absorbs) : base(int.MaxValue)
    {
        AbsorbsLeft = UnityEngine.Mathf.Max(0, absorbs);
        if (AbsorbsLeft <= 0)
            SetTurns(0);
    }
    
    public void AddAbsorbs(int absorbs)
    {
        absorbs = UnityEngine.Mathf.Max(0, absorbs);
        if (absorbs <= 0) return;

        // якщо щит був "мертвий" (turnsLeft=0) — оживляємо
        if (IsExpired)
            SetTurns(int.MaxValue);

        AbsorbsLeft += absorbs;
    }

    public bool TryAbsorbHit()
    {
        if (AbsorbsLeft <= 0) return false;

        AbsorbsLeft--;

        // ✅ щит не тікає по ходах, але має "вмерти", коли стаки 0
        if (AbsorbsLeft <= 0)
            SetTurns(0);

        return true;
    }

    // щит не тікає по ходах
    public override void OnTurnEnd(StatusController target) { }
}

public class ShieldStatus : StatusEffect
{
    public int AbsorbsLeft { get; private set; }

    public ShieldStatus(int absorbs) : base(int.MaxValue)
    {
        AbsorbsLeft = UnityEngine.Mathf.Max(0, absorbs);
    }

    public bool TryAbsorbHit()
    {
        if (AbsorbsLeft <= 0) return false;

        AbsorbsLeft--;
        if (AbsorbsLeft <= 0)
            RefreshToAtLeast(0); // робить IsExpired=true

        return true;
    }

    // щит не тікає по ходах
    public override void OnTurnEnd(StatusController target) { }
}

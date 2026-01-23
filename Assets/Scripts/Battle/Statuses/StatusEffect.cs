public abstract class StatusEffect
{
    public int turnsLeft { get; private set; }

    protected StatusEffect(int turns)
    {
        turnsLeft = turns;
    }

    public void RefreshToAtLeast(int turns)
    {
        if (turns > turnsLeft)
            turnsLeft = turns;
    }

    public void AddTurns(int extraTurns)
    {
        if (extraTurns <= 0) return;
        turnsLeft += extraTurns;
    }

    protected void SetTurns(int turns)
    {
        turnsLeft = turns;
    }

    public virtual void OnApply(StatusController target) { }
    public virtual void OnTurnStart(StatusController target) { }

    public virtual void OnTurnEnd(StatusController target)
    {
        turnsLeft--;
    }

    public bool IsExpired => turnsLeft <= 0;
}

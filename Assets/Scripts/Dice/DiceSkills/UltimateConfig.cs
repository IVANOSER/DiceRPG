using System;

[Serializable]
public class UltimateConfig
{
    public ChargeConfig charge = new ChargeConfig();
    public AoEConfig aoe = new AoEConfig();
    public SingleConfig single = new SingleConfig();
    public SelfConfig self = new SelfConfig();
}

[Serializable]
public class ChargeConfig
{
    public int maxCharge = 100;
    public int chargePerAttack = 25;
}

[Serializable]
public class AoEConfig
{
    public int damage = 8;
    public int burnTurns = 2;
    public int burnDamagePerTurn = 3;
    public int stunTurns = 1;
}

[Serializable]
public class SingleConfig
{
    public int damage = 18;
    public int burnTurns = 2;
    public int burnDamagePerTurn = 3;
    public int stunTurns = 1;
}

[Serializable]
public class SelfConfig
{
    public int healAmount = 0;
    public bool healFull = false;
    public int shieldAbsorbs = 3;
}

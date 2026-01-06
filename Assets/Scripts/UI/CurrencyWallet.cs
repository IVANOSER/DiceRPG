using System;
using UnityEngine;

public class CurrencyWallet : MonoBehaviour
{
    public static CurrencyWallet Instance { get; private set; }

    [SerializeField] private int gold = 0;
    [SerializeField] private int gems = 0;

    public int Gold => gold;
    public int Gems => gems;

    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemsChanged;

    private const string GoldKey = "WALLET_GOLD";
    private const string GemsKey = "WALLET_GEMS";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    public void AddGold(int amount)
    {
        if (amount == 0) return;
        gold = Mathf.Max(0, gold + amount);
        SaveGold();
        OnGoldChanged?.Invoke(gold);
    }

    public void AddGems(int amount)
    {
        if (amount == 0) return;
        gems = Mathf.Max(0, gems + amount);
        SaveGems();
        OnGemsChanged?.Invoke(gems);
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= 0) return true;
        if (gold < amount) return false;

        gold -= amount;
        SaveGold();
        OnGoldChanged?.Invoke(gold);
        return true;
    }

    public bool TrySpendGems(int amount)
    {
        if (amount <= 0) return true;
        if (gems < amount) return false;

        gems -= amount;
        SaveGems();
        OnGemsChanged?.Invoke(gems);
        return true;
    }

    public void SetGold(int value)
    {
        gold = Mathf.Max(0, value);
        SaveGold();
        OnGoldChanged?.Invoke(gold);
    }

    public void SetGems(int value)
    {
        gems = Mathf.Max(0, value);
        SaveGems();
        OnGemsChanged?.Invoke(gems);
    }

    private void Load()
    {
        gold = PlayerPrefs.GetInt(GoldKey, gold);
        gems = PlayerPrefs.GetInt(GemsKey, gems);
    }

    private void SaveGold() => PlayerPrefs.SetInt(GoldKey, gold);
    private void SaveGems() => PlayerPrefs.SetInt(GemsKey, gems);
}

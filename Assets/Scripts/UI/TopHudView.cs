using TMPro;
using UnityEngine;

public class TopHudView : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text gemsText;

    private void OnEnable()
    {
        // Якщо Wallet ще не в сцені — просто не падаємо.
        if (CurrencyWallet.Instance == null) return;

        CurrencyWallet.Instance.OnGoldChanged += UpdateGold;
        CurrencyWallet.Instance.OnGemsChanged += UpdateGems;

        // первинне заповнення
        UpdateGold(CurrencyWallet.Instance.Gold);
        UpdateGems(CurrencyWallet.Instance.Gems);
    }

    private void OnDisable()
    {
        if (CurrencyWallet.Instance == null) return;

        CurrencyWallet.Instance.OnGoldChanged -= UpdateGold;
        CurrencyWallet.Instance.OnGemsChanged -= UpdateGems;
    }

    private void UpdateGold(int value) => goldText.text = value.ToString();
    private void UpdateGems(int value) => gemsText.text = value.ToString();
}

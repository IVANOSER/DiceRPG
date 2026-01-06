using System.Collections;
using TMPro;
using UnityEngine;

public class TopHudView : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text gemsText;

    private CurrencyWallet wallet;

    private void OnEnable()
    {
        StartCoroutine(BindWhenReady());
    }

    private void OnDisable()
    {
        Unbind();
    }

    private IEnumerator BindWhenReady()
    {
        // чекаємо, поки CurrencyWallet з'явиться (навіть якщо створюється в іншій сцені/пізніше)
        while (CurrencyWallet.Instance == null)
            yield return null;

        Bind(CurrencyWallet.Instance);
    }

    private void Bind(CurrencyWallet w)
    {
        if (wallet == w) return;

        Unbind();
        wallet = w;

        wallet.OnGoldChanged += UpdateGold;
        wallet.OnGemsChanged += UpdateGems;

        // первинне заповнення
        UpdateGold(wallet.Gold);
        UpdateGems(wallet.Gems);
    }

    private void Unbind()
    {
        if (wallet == null) return;

        wallet.OnGoldChanged -= UpdateGold;
        wallet.OnGemsChanged -= UpdateGems;
        wallet = null;
    }

    private void UpdateGold(int value)
    {
        if (goldText) goldText.text = value.ToString();
    }

    private void UpdateGems(int value)
    {
        if (gemsText) gemsText.text = value.ToString();
    }
}

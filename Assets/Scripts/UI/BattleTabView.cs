using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleTabView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text levelNameText;
    [SerializeField] private Button fightButton;

    [Header("Scene")]
    [SerializeField] private string battleSceneName = "BattleScene";

    public void OnShow()
    {
        if (levelNameText) levelNameText.text = "Training Dummy";

        if (fightButton)
        {
            fightButton.interactable = true;

        }
    }

    public void OnHide() { }

    public void OnFightClicked()
    {
        SceneManager.LoadScene(battleSceneName);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenuUI : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject exitPopup;

    [Header("Menu scene")]
    [SerializeField] private string menuSceneName = "MainMenu";

    // Викликаєш з кнопки "Exit" (іконка/кнопка на екрані)
    public void OpenExitPopup()
    {
        if (exitPopup != null) exitPopup.SetActive(true);
        Time.timeScale = 0f; // пауза
    }

    // Кнопка "Ні"
    public void CloseExitPopup()
    {
        if (exitPopup != null) exitPopup.SetActive(false);
        Time.timeScale = 1f; // продовжити
    }

    // Кнопка "Так"
    public void ConfirmExitToMenu()
    {
        Time.timeScale = 1f; // важливо перед лоадом сцени
        SceneManager.LoadScene(menuSceneName);
    }
}

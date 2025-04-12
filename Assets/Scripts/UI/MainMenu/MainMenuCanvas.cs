using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private GameObject buttonsPanel;
    [SerializeField] private GameObject helpPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject aboutPanel;

    private void Start()
    {
        buttonsPanel.SetActive(true);
        helpPanel.SetActive(false);
        settingsPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    private void OpenNextPanel(GameObject panelToOpen)
    {
        buttonsPanel.SetActive(false);
        panelToOpen.SetActive(true);
    }

    #region MAIN MENU BUTTONS CALLBACKS
    public void OnPressPlay() // Играть
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnPressHelp() // Справка
    {
        OpenNextPanel(helpPanel);
    }

    public void OnPressSettings() // Настройки
    {
        OpenNextPanel(settingsPanel);
    }

    public void OnPressAbout() // Авторы
    {
        OpenNextPanel(aboutPanel);
    }

    public void OnPressQuit() // Выход
    {
        Application.Quit();
    }

    #endregion
}

using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject helpPanel;

    private void OnEnable()
    {
        InitializePauseCanvas();
    }

    private void Start()
    {
        InitializePauseCanvas();
    }

    private void SwitchPanel(GameObject panelToOpen)
    {
        mainButtonsPanel.SetActive(false);
        panelToOpen.SetActive(true);
    }

    private void InitializePauseCanvas()
    {
        mainButtonsPanel.SetActive(true);
        settingsPanel.SetActive(false);
        helpPanel.SetActive(false);
    }

    public void OnPressResume()
    {
        Debug.LogWarning("Resume button was pressed!");
    }

    public void OnPressRestart()
    {
        GameManager.Instance.RestartLevel();
    }

    public void OnPressSettings()
    {
        SwitchPanel(settingsPanel);
    }

    public void OnPressHelp()
    {
        SwitchPanel(helpPanel);
    }

    public void OnPressQuit()
    {
        GameManager.Instance.LoadLevel(0);
    }
}

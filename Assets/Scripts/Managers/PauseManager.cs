using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuCanvas;

    private void Start()
    {
        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);
        else
            Debug.LogError("No pause canvas assigned!");
    }

    // обычный метод открытия-закрытия канваса паузы (костыльный дубликат в обход CallbackContext'а для использщования в других скриптах)
    public void OpenClosePauseMenu()
    {
        //Debug.Log($"Before: {pauseMenuCanvas.activeSelf}");
        bool isPauseMenuCanvasEnabled = pauseMenuCanvas.activeSelf;
        pauseMenuCanvas.SetActive(!isPauseMenuCanvasEnabled);
        //Debug.Log($"After: {pauseMenuCanvas.activeSelf}");
    }

    // коллбек на нажатие Esc
    public void OnOpenClosePauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OpenClosePauseMenu();
        }
    }
 
}

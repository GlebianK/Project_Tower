using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Õ≈ “–Œ√¿“‹, ¡À›“!

    [SerializeField] private string playerMovementActionMapName = "Movement";
    [SerializeField] private string uiActionMapName = "GameManager";

    private float fixedDeltaTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
        */
    }

    private void BlockPlayerMovement(GameObject player)
    {
        if (player.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
        {
            playerInput.SwitchCurrentActionMap(uiActionMapName);
        }
    }

    private void ActivatePlayerMovement(GameObject player)
    {
        if (player.TryGetComponent<PlayerInput>(out PlayerInput playerInput))
        {
            playerInput.SwitchCurrentActionMap(playerMovementActionMapName);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
    }
    
    public void RestartLevel()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void StopGameplay(GameObject player = null)
    {
        Time.timeScale = 0.0f;
        Time.fixedDeltaTime = 0.0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player != null)
        {
            BlockPlayerMovement(player);
        }
    }

    public void ReturnGameplay(GameObject player = null)
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale; ;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null)
        {
            ActivatePlayerMovement(player);
        }
    }
}

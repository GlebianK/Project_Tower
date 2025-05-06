using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // НЕ ТРОГАТЬ, БЛЭТ!

    [SerializeField] private string playerMovementActionMapName = "Movement";
    [SerializeField] private string uiActionMapName = "GameManager";

    [SerializeField] private bool isDevMode = false;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private float playerInitialHP = 100;
    [SerializeField] private int initialMedKitsAmount = 0;

    private float fixedDeltaTime;

    public bool IsDevMode => isDevMode;
    public float PlayerInitialHP => playerInitialHP;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Start()
    {
        // спавним игрока в нужно месте
        if (isDevMode)
        {
            CharacterController[] playerCC = FindObjectsByType<CharacterController>(FindObjectsSortMode.None);
            //Debug.LogWarning($"Контроллеров найдено: {playerCC.Length}");

            if (playerCC != null && playerCC.Length == 1)
            {
                if (playerSpawnPoint != null)
                {
                    playerCC[0].gameObject.transform.position = playerSpawnPoint.position;
                }

                if (playerCC[0].gameObject.TryGetComponent<UseItem>(out UseItem useItemComponent))
                {
                    for (int i = 1; i <= initialMedKitsAmount; i++)
                        useItemComponent.AddMedKit();
                }
            }
            else
            {
                Debug.LogError("MORE OR LESS THEN ONE CCs ON SCENE!");
            }
        }
        else
        {
            initialMedKitsAmount = 0;
            playerInitialHP = 100;
        }
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

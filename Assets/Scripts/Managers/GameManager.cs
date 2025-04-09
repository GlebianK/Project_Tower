using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float fixedDeltaTime;
    private void Awake()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
    }
    
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
    }
}

using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public UnityEvent LostGameEvent;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPlayerDied()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LostGameEvent.Invoke();
    }
}

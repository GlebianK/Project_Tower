using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public float Score { get; private set; }

    public UnityEvent LostGameEvent;
    public UnityEvent CompleteGameEvent;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    public void OnPlayerDied()
    {
        StopGameplay();

        LostGameEvent.Invoke();
    }

    public void OnPlayerReachedEnd()
    {
        StopGameplay();

        CompleteGameEvent.Invoke();
    }

    private void StopGameplay()
    {
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

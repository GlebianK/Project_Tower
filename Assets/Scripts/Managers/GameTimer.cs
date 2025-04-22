using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float initialTimeInSeconds;
    [SerializeField] private bool isScaled = false;
    public float Time { get; private set; }

    private void Awake()
    {
        Time = initialTimeInSeconds;
    }
    private void Update()
    {
        Time -= isScaled ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime; //* timeScale;
        Time = Mathf.Clamp(Time, 0, initialTimeInSeconds);
    }

    public void RefreshTime()
    {
        Time = initialTimeInSeconds;
    }
}

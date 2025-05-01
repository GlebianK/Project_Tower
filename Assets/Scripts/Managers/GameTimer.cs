using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float initialTimeInSeconds;
    [SerializeField] private bool isScaled = false;
    public float Time { get; private set; }
    public float TimeClamped { get; private set; }

    private void Awake()
    {
        RefreshTime();
    }
    private void Update()
    {
        Time -= isScaled ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime;
        TimeClamped = Time;
        if (Time < 0)
            TimeClamped = 0;
    }

    public void RefreshTime()
    {

        Time = initialTimeInSeconds;
        TimeClamped = Time;
    }
}

using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private bool isScaled = false;
    public float Time { get; private set; }

    private void Awake()
    {
        Time = 0;
    }
    private void Update()
    {
        float timeScale = isScaled ? UnityEngine.Time.timeScale : 1;
        Time += UnityEngine.Time.unscaledDeltaTime * timeScale;
    }

    public void RefreshTime()
    {
        Time = 0;
    }
}

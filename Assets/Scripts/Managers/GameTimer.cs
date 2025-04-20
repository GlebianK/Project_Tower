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
        Time += isScaled ? UnityEngine.Time.deltaTime : UnityEngine.Time.unscaledDeltaTime; //* timeScale;
    }

    public void RefreshTime()
    {
        Time = 0;
    }
}

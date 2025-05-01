using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EndStatsScreen : MonoBehaviour
{
    [SerializeField] private GameplayManager gameplayManager;
    [SerializeField] private GameTimer timer;
    [Header("UI")]
    [SerializeField] private TMP_Text killView;
    [SerializeField] private TMP_Text healthKitView;
    [SerializeField] private TMP_Text timeView;
    private string format = "{0:D2}:{1:D2}:{2:D2}";

    private void OnEnable()
    {
        UpdateView();
    }

    public void UpdateView()
    {
        float gameTime = timer.TimeClamped;
        int minute = Mathf.FloorToInt(gameTime / 60);
        int second = Mathf.FloorToInt(gameTime) - minute * 60;

        int millisec = Mathf.FloorToInt(Mathf.Max((int)(timer.Time * 1000) % 1000, 0));
        timeView.text = string.Format(format, minute, second, millisec);

        killView.text = gameplayManager.GetEnemyKillsCounted().ToString();
        healthKitView.text = gameplayManager.HealthkitCollected.ToString();
    }
}

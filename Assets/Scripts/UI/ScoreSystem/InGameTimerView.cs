using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class InGameTimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private GameTimer timerHandler;
    [SerializeField] private string format;

    private void Update()
    {
        if (timerHandler.enabled)
        {
            int minute = Mathf.FloorToInt(timerHandler.Time / 60);
            int second = Mathf.FloorToInt(timerHandler.Time) - minute * 60;
            int millisec = Mathf.FloorToInt((timerHandler.Time * 1000) % 1000);
            textField.text = string.Format(format, minute, second, millisec);
        }
    }

    private void OnValidate()
    {
        if (textField == null) return;

        textField.text = string.Format(format, 0, 0, 0); ;
    }
}

using TMPro;
using UnityEngine;

public class InGameTimerView : MonoBehaviour
{
    [SerializeField] private TMP_Text textField;
    [SerializeField] private GameTimer timerHandler;
    [SerializeField] private string format;

    private void Update()
    {
        if (timerHandler.enabled)
        {
            float gameTime = timerHandler.TimeClamped;
            int minute = Mathf.FloorToInt(gameTime / 60);
            int second = Mathf.FloorToInt(gameTime) - minute * 60;

            int millisec = Mathf.FloorToInt(Mathf.Max((int)(timerHandler.Time * 1000) % 1000, 0));
            textField.text = string.Format(format, minute, second, millisec);
        }
    }

    private void OnValidate()
    {
        if (textField == null) return;

        textField.text = string.Format(format, 0, 0, 0); 
    }
}

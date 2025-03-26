using TMPro;
using UnityEngine;

public class UIKitCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;
    [SerializeField] private MedKitCounter counterMedKit;
    public void AddMedKitText()
    {
        counterText.text = counterMedKit.MedKitTotal.ToString();
    }
    public void RemoveMedKitText()
    {
        counterText.text = counterMedKit.MedKitTotal.ToString();
    }
}

using UnityEngine;
using UnityEngine.Events;

public class MedKitCounter : MonoBehaviour
{
    public UnityEvent AddMedKitInText;
    public UnityEvent RemoveMedKitInText;

    [SerializeField] private int medKitTotal = 0;
    public int MedKitTotal => medKitTotal;
    public void AddMedKit()
    {
        medKitTotal += 1;
        AddMedKitInText.Invoke();
        Debug.Log("AddMedKit");
    } 
    public void RemoveMedKit()
    {
        medKitTotal -= 1;
        RemoveMedKitInText.Invoke();
        Debug.Log("RemoveMedKit");
    }
}

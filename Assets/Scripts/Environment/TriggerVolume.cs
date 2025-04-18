using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerVolume : MonoBehaviour
{
    [SerializeField] private bool deactivateOnEntry = false;

    public UnityEvent TriggerEntered;
    public UnityEvent TriggerStayed;
    public UnityEvent TriggerExited;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEntered?.Invoke();
        if (deactivateOnEntry)
            enabled = false;
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerStayed?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExited?.Invoke();
    }
}

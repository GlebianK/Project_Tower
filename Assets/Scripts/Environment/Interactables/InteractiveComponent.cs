using UnityEngine;

public class InteractiveComponent : MonoBehaviour
{
    [SerializeField] private string objectName;
    [SerializeField] private string actionName;

    public string GetObjectName() => objectName;
    public string GetActionName() => actionName;

    public virtual void Interact()
    {
        Debug.LogWarning("A basic implementation is used instead of a specific one");
    }
    public virtual void Deinteract()
    {
        Debug.LogWarning("A basic implementation is used instead of a specific one");
    }
    public virtual void InteractionAction()
    {
        Debug.LogWarning("A basic implementation is used instead of a specific one");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Deinteract();
        }
    }
}

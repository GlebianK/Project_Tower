using UnityEngine;

public class InteractiveComponent : MonoBehaviour
{
    [SerializeField] private string objectName;
    [SerializeField] private string actionName;

    public string GetObjectName() => objectName;
    public string GetActionName() => actionName;


    public virtual void Interact()
    {
        Debug.LogWarning("A basic implementation of Interact is used instead of a specific one");
        
    }

    public virtual void Deinteract()
    {
        Debug.LogWarning("A basic implementation of Deinteract is used instead of a specific one");
        
    }

    public virtual void InteractionAction(GameObject player)
    {
        Debug.LogWarning("A basic implementation of InteractionAction is used instead of a specific one");
    }
}

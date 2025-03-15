using UnityEngine;

public class InteractiveComponent : MonoBehaviour
{
    [SerializeField] private string objectName;
    [SerializeField] private string actionName;

    protected Collider playerCollider;

    public string GetObjectName() => objectName;
    public string GetActionName() => actionName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCollider = other;
            Interact();
            Debug.Log($"Hello, player! Collider: {playerCollider}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Deinteract();
            playerCollider = null;
            Debug.Log($"Bye, player! Collider: {playerCollider}");
        }
    }

    public virtual void Interact()
    {
        Debug.LogWarning("A basic implementation is used instead of a specific one");
        if (playerCollider != null)
        {
            if (playerCollider.TryGetComponent<PlayerInteraction>(out PlayerInteraction playerInteractionComponent))
            {
                playerInteractionComponent.SetObjectOfInteraction(this);
                playerInteractionComponent.InteractionStarted.Invoke();
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Unable to interact!");
        }
    }

    public virtual void Deinteract()
    {
        Debug.LogWarning("A basic implementation is used instead of a specific one");
        if (playerCollider != null)
        {
            if (playerCollider.TryGetComponent<PlayerInteraction>(out PlayerInteraction playerInteractionComponent))
            {
                playerInteractionComponent.InteractionEnded.Invoke();
                playerInteractionComponent.ResetObjectOfInteraction();
            }
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: Unable to deinteract!");
        }
    }

    public virtual void InteractionAction()
    {
        Debug.LogWarning("A basic implementation is used instead of a specific one");
    }
}

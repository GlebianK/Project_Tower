using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private InteractionController objectOfInteraction;
    private string interactiveObjectName;

    public UnityEvent InteractionStarted;
    public UnityEvent InteractionEnded;
    public UnityEvent MedKitAdd;
    private void Awake()
    {
        objectOfInteraction = null;
        interactiveObjectName = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectable") || other.CompareTag("Interactable"))
        {
            if (other.TryGetComponent<InteractionController>(out objectOfInteraction))
            {
                Debug.Log($"Successfully extracted IC from: {other.gameObject.name}");
                interactiveObjectName = other.gameObject.name;
                Interact();
            }
            else
            {
                Debug.LogWarning($"Couldn't extract IC from: {other.gameObject.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectable") || other.CompareTag("Interactable"))
        {
            if (objectOfInteraction != null && other.gameObject.name == interactiveObjectName)
            {
                ForgetObjectOfInteraction();
            }
            Debug.Log($"Successfully dropped IC from: {other.gameObject.name}");
        }
    }

    private void Interact()
    {
        if (objectOfInteraction != null)
        {
            InteractionStarted.Invoke();
            objectOfInteraction.Interact();
        }
    }

    private void Deinteract()
    {
        if (objectOfInteraction != null)
        {
            InteractionEnded.Invoke();
            objectOfInteraction.Deinteract();
        }
    }

    private void InteractionAction()
    {
        if (objectOfInteraction != null)
        {
            InteractionEnded.Invoke();
            objectOfInteraction.InteractionAction();
            ForgetObjectOfInteraction();
        }
    }

    private void ForgetObjectOfInteraction()
    {
        Deinteract();
        interactiveObjectName = "";
        objectOfInteraction = null;
    }

    public void OnInteract(InputAction.CallbackContext context) 
    {
        InteractionAction();
    }
}

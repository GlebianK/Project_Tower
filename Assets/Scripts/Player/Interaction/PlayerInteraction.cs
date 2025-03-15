using UnityEngine;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    private InteractiveComponent objectOfInteraction;

    public UnityEvent InteractionStarted;
    public UnityEvent InteractionEnded;

    public void SetObjectOfInteraction(InteractiveComponent objectToInteract)
    {
        objectOfInteraction = objectToInteract;
    }

    public void ResetObjectOfInteraction()
    {
        objectOfInteraction = null;
    }
}

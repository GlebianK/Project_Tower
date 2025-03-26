using TMPro;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    [SerializeField] private InteractiveComponent interComp;
    public void Interact()
    {
        interComp.Interact();
    }
    public void Deinteract()
    {
        interComp.Deinteract();
    }
    public void InteractionAction()
    {
        interComp.InteractionAction();
    }
}

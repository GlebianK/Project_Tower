using TMPro;
using UnityEngine;

public class InteractMedKit : MonoBehaviour
{
    [SerializeField] private TMP_Text interactText;

    public void InteractionStarted()
    {
        interactText.enabled = true;
    }
    public void InteractionEnded()
    {
        interactText.enabled = false;
    }
}

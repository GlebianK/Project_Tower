using UnityEngine;
using UnityEngine.InputSystem;

public class UseMedKit : MonoBehaviour
{
    [SerializeField] private MedKitCounter medKitCounter;
    [SerializeField] private Health health;
    [SerializeField] private float healthToAdd;
    private bool canUseMedKit = true;
    public void OnHeal(InputAction.CallbackContext context)
    {
        if (health != null && medKitCounter != null)
        {
            if (medKitCounter.MedKitTotal > 0 && context.started)
            {
                canUseMedKit = false;
                health.Heal(healthToAdd);
                medKitCounter.RemoveMedKit();
                Debug.Log("Used Health item");
            }
            else
            {
                if(context.started)
                    Debug.Log("Dont have health item ");
            }
        }
        else
        {
            Debug.LogWarning("Components not found");
        }
    }
}

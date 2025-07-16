using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFlashlightInput : MonoBehaviour
{
    [SerializeField] private PlayerFlashlight playerFlashlight;

    public void OnFlashlightToggleInput(InputAction.CallbackContext context)
    {
        if (context.performed) 
        { 
            if (playerFlashlight != null) 
            {
                playerFlashlight.Toggle();
            }
            else
            {
                Debug.LogError("PLayerFlashlightInput Error: playerFlashlight is not provided");
            }
        }
    }
}

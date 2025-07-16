using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    [Header("Sound Effects")]
    [SerializeField] private PlayerSFXCustomizer playerSFXCustomizer;
    [SerializeField] private string sfxShotName;

    private void Start()
    {
        flashlight.enabled = false;
    }

    public void Toggle()
    {
        flashlight.enabled = !flashlight.enabled;
        playerSFXCustomizer.PlayOneShot(sfxShotName);
    }
}

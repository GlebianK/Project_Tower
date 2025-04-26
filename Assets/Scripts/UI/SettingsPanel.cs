using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;

    //Callbacks for Sliders
    public void OnChangeMusicMasterVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ChangeMusicMasterVolume(musicVolume.value);
        }
        else
        {
            Debug.LogError("No audio manager to save new settings!");
        }
    }

    public void OnChangeSFXMasterVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ChangeSFXMasterVolume(sfxVolume.value);
        }
        else
        {
            Debug.LogError("No audio manager to save new settings!");
        }
    }
}

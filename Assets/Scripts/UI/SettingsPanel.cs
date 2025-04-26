using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider sfxVolume;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            musicVolume.value = AudioManager.Instance.GetCurrentMusicMasterVolume();
            sfxVolume.value = AudioManager.Instance.GetCurrentSFXMasterVolume();
        }
        else
        {
            Debug.LogError("Settings Panel: CANNOT FIND AUDIO MANAGER INSTANCE");
        }
    }

    //Callbacks for Sliders
    public void OnChangeMusicMasterVolume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ChangeMusicMasterVolume(musicVolume.value);
            AudioManager.Instance.MusicVolumeChanged.Invoke();
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
            AudioManager.Instance.SfxVolumeChanged.Invoke();
        }
        else
        {
            Debug.LogError("No audio manager to save new settings!");
        }
    }
}

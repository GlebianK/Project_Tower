using UnityEngine;

public class SoundSystemAmbient : SoundSystemBase
{
    [SerializeField] protected AudioSource musicSource;

    private SoundItem currentMusicItem;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MusicVolumeChanged.AddListener(OnMusicVolumeChanged);
            AudioManager.Instance.SfxVolumeChanged.AddListener(OnSFXVolumeChanged);
            InitializeSources();
        }
        else
        {
            Debug.LogError("SSA -> Start. Cannot find AM!");
        }
    }

    private void OnDisable()
    {
        AudioManager.Instance.MusicVolumeChanged.RemoveListener(OnMusicVolumeChanged);
    }

    private void InitializeSources()
    {
        Debug.LogWarning($"{gameObject.name}'s sound INIT method");

        if (sfxSource != null && AudioManager.Instance != null)
        {
            sfxSource.volume = AudioManager.Instance.GetCurrentSFXMasterVolume();
        }
        else
        {
            Debug.LogError($"{gameObject.name}: SFX source or AM.Instance is NULL!");
            return;
        }

        if (musicSource != null)
        {
            musicSource.volume = AudioManager.Instance.GetCurrentMusicMasterVolume();
        }
        else
        {
            Debug.LogError($"{gameObject.name}: MUSIC source is NULL!");
            return;
        }

        if (soundsCollection != null)
        {
            if (soundsCollection.Length == 1)
            {
                currentMusicItem = soundsCollection[0];
                musicSource.clip = currentMusicItem.clip;
                musicSource.loop = currentMusicItem.isLooped;
                musicSource.Play();
            }
            else if (soundsCollection.Length == 2)
            {
                currentSFXItem = soundsCollection[0];
                sfxSource.clip = currentSFXItem.clip;
                sfxSource.loop = currentSFXItem.isLooped;
                sfxSource.Play();

                currentMusicItem = soundsCollection[1];
                musicSource.clip = currentMusicItem.clip;
                musicSource.loop = currentMusicItem.isLooped;
                musicSource.Play();
            }
            else
                Debug.LogError($"SSA: soundsCollection Length = {soundsCollection.Length} is invalid!");
        }
    }
    

    public void OnMusicVolumeChanged()
    {
        if (currentMusicItem != null)
            musicSource.volume = currentMusicItem.volume * AudioManager.Instance.GetCurrentMusicMasterVolume();
        else
        {
            Debug.LogWarning("SSA music callback: no item to extract volume from!");
            return;
        }
    }
}

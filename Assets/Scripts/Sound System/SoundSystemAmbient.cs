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

    protected override void InitializeSources()
    {
        Debug.LogWarning($"{gameObject.name}'s sound INIT method");
        base.InitializeSources();

        if (musicSource != null)
        {
            musicSource.volume *= AudioManager.Instance.GetCurrentMusicMasterVolume();
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
                musicSource.clip = soundsCollection[0].clip;
                if(musicSource.volume > 0)
                    musicSource.Play();
            }
            else
            {
                currentSFXItem = soundsCollection[0];
                sfxSource.clip = soundsCollection[0].clip;
                sfxSource.Play();

                currentMusicItem = soundsCollection[1];
                musicSource.clip = soundsCollection[1].clip;
                musicSource.Play();
            }
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

        /*
        if (musicSource.volume == 0)
            musicSource.Stop();
        else if (!musicSource.isPlaying)
            musicSource.Play();
        */
    }
}

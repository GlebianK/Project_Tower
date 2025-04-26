using System.Collections.Generic;
using UnityEngine;

public class SoundSystemBase : MonoBehaviour
{
    [Tooltip("Для эмбиент-объекта: элемент 0 - эмбиент, элемент 1 - музыка")]
    [SerializeField] protected SoundItem[] soundsCollection;
    [SerializeField] protected AudioSource sfxSource;

    protected SoundItem currentSFXItem;
    protected Dictionary<string, AudioClip> soundsDict;

    private void Awake()
    {
        soundsDict = new();

        if (soundsCollection != null && soundsCollection.Length > 0)
        {
            foreach (SoundItem sound in soundsCollection)
            {
                soundsDict.Add(sound.action, sound.clip);
            }
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Something is wrong with audio collection!");
        }
    }

    private void Start()
    {
        AudioManager.Instance.SfxVolumeChanged.AddListener(OnSFXVolumeChanged);
        InitializeSources();
    }

    private void OnDisable()
    {
        AudioManager.Instance.SfxVolumeChanged.RemoveListener(OnSFXVolumeChanged);
    }

    // Базовый метод инициализации источника(-ов) звука. SFX есть у всех, потому его громкость инициализируем сразу. Нюансы - в наследниках
    protected virtual void InitializeSources()
    {
        Debug.LogWarning("Base INIT method of sound system");
        if (sfxSource != null)
        {
            sfxSource.volume *= AudioManager.Instance.GetCurrentSFXMasterVolume();
        }
        else
        {
            Debug.LogError($"{gameObject.name}: SFX source is NULL!");
            return;
        }
    }

    public void OnSFXVolumeChanged()
    {
        if (currentSFXItem != null)
            sfxSource.volume = currentSFXItem.volume * AudioManager.Instance.GetCurrentSFXMasterVolume();
        else
            Debug.LogWarning("SSB sfx callback: no item to extract volume from!");
    }
}

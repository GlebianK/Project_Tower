/*
 * Данный скрипт отвечает за хранение, изменение и подгрузку настроек звука в игре
 * Данный скрипт должен быть прикреплён к префаб-объекту AudioManager
 * Данный скрипт реализует паттерн синглтон для простоты и удобства доступа к управляющим методам 
 * Данный скрипт реализует сохранение и загрузку данных настроек звука напрямую через PlayerPrefs
 */

using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Tooltip("0 - без звука, 1 - громкость 100%")]
    [SerializeField, Range(0f, 1f)] private float defaultMusicMasterVolume = 0.55f;

    [Tooltip("0 - без звука, 1 - громкость 100%")]
    [SerializeField, Range(0f, 1f)] private float defaultSFXMasterVolume = 0.85f;

    private float currentMusicMasterVolume;
    private float currentSFXMasterVolume;

    public UnityEvent MusicVolumeChanged;
    public UnityEvent SfxVolumeChanged;

    public float GetCurrentMusicMasterVolume() => currentMusicMasterVolume;
    public float GetCurrentSFXMasterVolume() => currentSFXMasterVolume;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeSoundSettings();
    }

    private void InitializeSoundSettings()
    {
        Debug.Log("Audio Manager: Loading data...");

        if (PlayerPrefs.HasKey("MusicMasterVolume"))
        {
            currentMusicMasterVolume = PlayerPrefs.GetFloat("MusicMasterVolume");
            Debug.Log($"Found saved music master volume value: {currentMusicMasterVolume}");
        }
        else
        {
            currentMusicMasterVolume = defaultMusicMasterVolume;
            PlayerPrefs.SetFloat("MusicMasterVolume", currentMusicMasterVolume);
            Debug.Log($"Applying default music master volume value: {currentMusicMasterVolume}");
        }

        if (PlayerPrefs.HasKey("MusicSFXVolume"))
        {
            currentSFXMasterVolume = PlayerPrefs.GetFloat("MusicSFXVolume");
            Debug.Log($"Found saved sfx master volume value: {currentSFXMasterVolume}");
        }
        else
        {
            currentSFXMasterVolume = defaultSFXMasterVolume;
            PlayerPrefs.SetFloat("MusicSFXVolume", currentSFXMasterVolume);
            Debug.Log($"Applying default sfx master volume value: {currentSFXMasterVolume}");
        }

        Debug.Log("AudioManager: Sound settings initiated!");
    }

    public void ChangeMusicMasterVolume(float newValue)
    {
        currentMusicMasterVolume = newValue;
        PlayerPrefs.SetFloat("MusicMasterVolume", currentMusicMasterVolume);
    }

    public void ChangeSFXMasterVolume(float newValue)
    {
        currentSFXMasterVolume = newValue;
        PlayerPrefs.SetFloat("MusicSFXVolume", currentSFXMasterVolume);
    }
}

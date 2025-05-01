/*
 * Базовый скрипт компонента звуковой системы
 * Уже в текущем виде может быть использован как полноценный компонент в префабах
 * Реализует работу с SFX составляющей. Работа с музыкой реализуется дополнительно в эмбиент-наследнике
 * Одним из protected полей является словарь "действие-клип". Рекомендуется использовать его для доступа к элементам коллекции
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystemBase : MonoBehaviour
{
    [Tooltip("Для эмбиент-объекта: элемент 0 - эмбиент, элемент 1 - музыка")]
    [SerializeField] protected SoundItem[] soundsCollection;
    [SerializeField] protected AudioSource sfxSource;

    protected float sfxMasterVolume;
    protected SoundItem currentSFXItem; // наследуемый член для текущего sfx-объекта типа SoundItem (дб у всех, кто реализует SoundSystem*)
    protected Dictionary<string, SoundItem> soundsDict; // словарь "действие-клип" для быстроты поиска объектов коллекции

    private void Awake()
    {
        soundsDict = new();
        currentSFXItem = null;
        sfxSource.playOnAwake = false;

        if (soundsCollection != null && soundsCollection.Length > 0)
        {
            foreach (SoundItem sound in soundsCollection)
            {
                soundsDict.Add(sound.action, sound);
            }
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Something is wrong with audio collection!");
        }
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SfxVolumeChanged.AddListener(OnSFXVolumeChanged);
            sfxMasterVolume = AudioManager.Instance.GetCurrentSFXMasterVolume();
        }
        else
        {
            Debug.LogError($"{gameObject.name}: AM.Instance is NULL!");
        }
    }

    private void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SfxVolumeChanged.RemoveListener(OnSFXVolumeChanged);
    }
    
    public virtual void PlaySFXSound(string actionName)
    {
        if (soundsDict.TryGetValue(actionName, out SoundItem item))
        {
            currentSFXItem = item;
            sfxSource.volume = currentSFXItem.volume * sfxMasterVolume;
            sfxSource.clip = currentSFXItem.clip;
            sfxSource.loop = currentSFXItem.isLooped;
            sfxSource.pitch = currentSFXItem.pitch;
            StartCoroutine(PlaySoundDelayCoroutine(sfxSource, currentSFXItem.delayTimer));
            //sfxSource.Play();
        }
        else
            Debug.LogError($"No action named {actionName}!!!");
    }

    private IEnumerator PlaySoundDelayCoroutine(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (sfxSource.isActiveAndEnabled)
            sfxSource.Play();
        yield return null;
    }

    // Callback for sfx volume change in Settings panel
    public void OnSFXVolumeChanged()
    {
        if (currentSFXItem != null && AudioManager.Instance != null)
        {
            sfxMasterVolume = AudioManager.Instance.GetCurrentSFXMasterVolume();
            sfxSource.volume = currentSFXItem.volume * sfxMasterVolume;
        }
        else
            Debug.LogWarning($"{gameObject.name}: SSB sfx callback: sfxItem or AM.Instance is null!");
    }
}

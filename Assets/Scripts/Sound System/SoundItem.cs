using UnityEngine;

[System.Serializable]
public class SoundItem
{
    public string action;
    public AudioClip clip;
    public bool isLooped;

    [Tooltip("Настраивать этот параметр при громкостях в AudioManager, равных 1. В дальнейшем именно Master Volume будет влиять на громкость")]
    [Range(0f, 1f)] public float volume;

    [Tooltip("Влияет на скорость воспроизведения аудиоклипа")]
    [Range(-3f, 3f)] public float pitch;

}

using UnityEngine;

[System.Serializable]
public class SoundItem
{
    public string action;
    public AudioClip clip;

    [Tooltip("Настраивать этот параметр при громкостях в AudioManager, равных 1. В дальнейшем именно Master Volume будет влиять на громкость")]
    [Range(0f, 1f)] public float volume;
}

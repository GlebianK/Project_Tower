using UnityEngine;

public class SoundSystemRaider : SoundSystemBase
{
    /*
    public override void PlaySFXSound(string actionName)
    {
        if (soundsDict.TryGetValue(actionName, out SoundItem item))
        {
            currentSFXItem = item;
            sfxSource.volume = currentSFXItem.volume * sfxMasterVolume;
            sfxSource.clip = currentSFXItem.clip;
            sfxSource.loop = currentSFXItem.isLooped;
            sfxSource.pitch = currentSFXItem.pitch;
            sfxSource.Play();
        }
        else
            Debug.LogError($"No action named {actionName}!!!");
    }
    */
}

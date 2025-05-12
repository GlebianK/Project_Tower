using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// ПОКА НЕ РАБОТАЕТ ПРОТИВ ВРАГОВ, ВИДИМО, ЧТО-ТО С ТРИГГЕРАМИ, НАДО ТЕСТИРОВАТЬ ДАЛЬШЕ
public class DangerousZone : MonoBehaviour
{
    [SerializeField] private float frequencyOfAttacks;
    [SerializeField] private float damage;
    [SerializeField] private DangerType zoneType;

    private bool canAttack = true;
    private List<Health> objectsToDamage;

    private AudioSource audioSource;
    private SoundSystemBase soundSystem;

    public DangerType GetZoneType() => zoneType;

    public UnityEvent<DangerType> ObjectEntered;
    public UnityEvent<DangerType> ObjectExited;

    public enum DangerType
    {
        light,
        mud
    }

    private void Awake()
    {
        objectsToDamage = new List<Health>();

        if (!TryGetComponent<AudioSource>(out audioSource))
            Debug.LogError("No audio sources found!");

        if (!TryGetComponent<SoundSystemBase>(out soundSystem))
            Debug.LogError("No sound systems found!");
    }

    private void OnTriggerEnter(Collider other)
    {
        ObjectEntered.Invoke(zoneType);

        if (other.TryGetComponent<Health>(out Health healthComponent))
        {
            if (objectsToDamage.Contains(healthComponent))
            {
                return;
            }
            objectsToDamage.Add(healthComponent);

            Debug.Log($"{other.gameObject.name} is in the danger zone!");
        }

        // переключение звука с всплеска на шипение для грязи
        if (zoneType == DangerType.mud)
            StartCoroutine(SwitchClipToDamageCoroutine());
    }

    private void Update()
    {
        for (int i = 0; i < objectsToDamage.Count; i++)
        {
            if (objectsToDamage[i].gameObject.TryGetComponent<CharacterController>(out CharacterController cc))
            {
                if (cc.enabled == false)
                {
                    OnTriggerExit(cc);
                }
            }
        }

        if (objectsToDamage.Count > 0 && canAttack)
        {
            for (int i = 0; i < objectsToDamage.Count; i++)
            {
                if (objectsToDamage[i] != null)
                {
                    //Debug.Log("CC enabled!");
                    objectsToDamage[i].TakeDamage(damage);
                }
            }
            StartCoroutine(PerformAttackCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Something left the zone!");

        ObjectExited.Invoke(zoneType);

        if (other.TryGetComponent<Health>(out Health healthComponent))
        {
            if (objectsToDamage.Contains(healthComponent))
            {
                objectsToDamage.Remove(healthComponent);
                Debug.Log("Health component removed from List!");
            }

            Debug.Log($"{other.gameObject.name} is out of the zone");
        }
    }

    private IEnumerator PerformAttackCoroutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(frequencyOfAttacks);
        canAttack = true;
    }

    private IEnumerator SwitchClipToDamageCoroutine()
    {
        if (audioSource.clip != null)
            yield return null;
        while(audioSource.isPlaying)
            yield return null;
        if (soundSystem != null)
        {
            soundSystem.PlaySFXSound("Damage");
            float temp = audioSource.volume;
            audioSource.volume = 0f;
            audioSource.DOFade(temp, 0.5f); // 0.5 - хардкод ебаный, но мне уже лень выводить 100500 настроек в инспектор 
        }
        yield return null;
    }
}

using System;
using System.Linq;
using UnityEngine;

[Serializable]
public struct SFXOneshotsPlayer
{
    public string action;
    public AudioClip audioClip;
    public float volume;
}

public class PlayerSFXCustomizer : MonoBehaviour
{
    [SerializeField] private AudioSource mainSource;
    [SerializeField] private AudioSource oneshotSource;
    [Header("Dependencies")]
    [SerializeField] private PlayerMovementStateMachine movementController;
    [SerializeField] private Health health;
    [SerializeField] private SFXOneshotsPlayer[] shots;

    private float sfxMasterVolume;

    private float movestateVolumeModifier;
    private PlayerMovementStateType prevState;
    private void Awake()
    {
        mainSource.playOnAwake = false;
        oneshotSource.playOnAwake = false;
    }
    private void Start()
    {
        prevState = movementController.GetCurrentStateType();
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SfxVolumeChanged.AddListener(OnSFXVolumeChanged);
            OnSFXVolumeChanged();
        }
        else
        {
            Debug.LogError($"{gameObject.name}: AM.Instance is NULL!");
        }
    }
    private void Update()
    {
        if (prevState == PlayerMovementStateType.Walk ||
            prevState == PlayerMovementStateType.Crouch ||
            prevState == PlayerMovementStateType.Sprint ||
            prevState == PlayerMovementStateType.Hang)
        {
            Vector3 playerVelocity = movementController.CharacterVelocity;
            float playerStepMaxVolumeVelocityClamp = 5.0f;
            movestateVolumeModifier = Mathf.Clamp01(playerVelocity.magnitude / playerStepMaxVolumeVelocityClamp);
        }
        else
        {
            movestateVolumeModifier = 1;
        }

        mainSource.volume = sfxMasterVolume * movestateVolumeModifier;
        oneshotSource.volume = sfxMasterVolume;
    }

    private void OnEnable()
    {
        movementController.StateChanged.AddListener(OnMovementStateChanged);
        health.TookDamage.AddListener(OnTakeDamage);
    }
    private void OnDisable()
    {
        movementController.StateChanged.RemoveListener(OnMovementStateChanged);
        health.TookDamage.RemoveListener(OnTakeDamage);
    }

    private void OnDestroy()
    {

        if (AudioManager.Instance != null)
            AudioManager.Instance.SfxVolumeChanged.RemoveListener(OnSFXVolumeChanged);
    }

    private void OnSFXVolumeChanged()
    {
        if (AudioManager.Instance != null)
        {
            sfxMasterVolume = AudioManager.Instance.GetCurrentSFXMasterVolume();
        }
    }

    public void OnMovementStateChanged(PlayerMovementStateMachine machine, PlayerMovementStateType state)
    {
        if (state == PlayerMovementStateType.Walk ||
            state == PlayerMovementStateType.Crouch ||
            state == PlayerMovementStateType.Sprint)
        {
            if (prevState == PlayerMovementStateType.Air)
            {
                //PlayOneShot("Land");
            }
        }

        if (state == PlayerMovementStateType.Air && prevState != PlayerMovementStateType.Climb)
        {
            if (machine.CharacterVelocity.y > 0.3f)
                PlayOneShot("Jump");
        }

        if (state == PlayerMovementStateType.Dash)
        {
            PlayOneShot("Dash");
        }

        if (state == PlayerMovementStateType.Climb)
        {
            PlayOneShot("Climb_Ooah");
        }

        prevState = state;
    }

    public void PlayOneShot(string action)
    {
        SFXOneshotsPlayer actionParams = shots.Where(desc => desc.action == action).First();
        oneshotSource.PlayOneShot(actionParams.audioClip, actionParams.volume);
    }

    public void OnTakeDamage()
    {
        if (health.DamageReductionCoef > 0)
        {
            PlayOneShot("GetHit_Block");
        }
        else
        {
            PlayOneShot("GetHit");
        }
    }


}

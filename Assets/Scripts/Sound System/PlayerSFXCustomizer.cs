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
    [SerializeField] private PlayerMovementStateMachine movementController;

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
    }
    private void OnDisable()
    {
        movementController.StateChanged.RemoveListener(OnMovementStateChanged);
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
}

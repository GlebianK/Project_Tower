using UnityEngine;

public class NewEnemyShield : NewEnemyBase
{
    [SerializeField] private CombatAnimations combatAnimationsComponent;
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private AudioSource audioSourceMain;
    [SerializeField] private AudioSource audioSourceForShield;
    
    [Tooltip("Остаток здоровья для исчезновения щита (в процентах от максимального здоровья)")]
    [SerializeField, Range(0, 100)] private int dropShieldHPThreshold;

    private bool hasShield;
    private SoundSystemRaider soundSystemRaider;

    public bool HasShield() => hasShield;

    protected new void Awake()
    {
        base.Awake();
        rewardResolveType = EnemyType.Heavy;
        hasShield = true;

        if (gameObject.TryGetComponent<SoundSystemRaider>(out soundSystemRaider))
            health.TookDamage.AddListener(PlayShieldSound);
        else
            Debug.LogError($"{gameObject}, Awake: soundSystemRaider is null!");
    }

    private void Update()
    {
        if (isAlive)
            EnemyBehaviourCycle();
    }

    private void OnTriggerEnter(Collider other)
    {
        DetectPlayer(other);
    }

    private void OnTriggerExit(Collider other)
    {
        LosePlayer(other);
    }

    private void PlayShieldSound()
    {
        if (soundSystemRaider != null)
            soundSystemRaider.PlaySFXSound("HitOnShield");
        else
            Debug.LogError($"{gameObject}: soundSystemRaider is null!");
    }

    private void SubscribeEventsToLightDamageWithoutShield()
    {
        health.TookDamage.AddListener(combatController.OnAttackAborted);
        health.TookDamage.AddListener(combatAnimationsComponent.OnGetHit);
        health.TookDamage.AddListener(combatController.OnGetHit);
        health.TookDamage.AddListener(OnGetHit);

        health.TookDamage.RemoveListener(PlayShieldSound);
    }

    private void OnDisable()
    {
        health.TookDamage.RemoveListener(combatController.OnAttackAborted);
        health.TookDamage.RemoveListener(combatAnimationsComponent.OnGetHit);
        health.TookDamage.RemoveListener(combatController.OnGetHit);
        health.TookDamage.RemoveListener(OnGetHit);
    }

    public void DropShield()
    {
        if (hasShield)
        {
            if (TryGetComponent<Health>(out Health health))
            {
                Debug.Log($"Current Shield Enemy health: {health.HP}. Threshold: {health.MaxHP * (dropShieldHPThreshold / 100f)}");
                if (health.HP > (health.MaxHP * (dropShieldHPThreshold / 100f)))
                    return;

                if (shieldObject.activeSelf)
                    shieldObject.SetActive(false);

                hasShield = false;

                if (audioSourceForShield != null && audioSourceForShield.clip != null)
                {
                    audioSourceForShield.volume = 0.5f * AudioManager.Instance.GetCurrentSFXMasterVolume();
                    audioSourceForShield.Play();
                }
                else
                    Debug.LogError($"{gameObject.name}: SOMETHING IS WRONG WITH SHIELD SOURCE!");

                SubscribeEventsToLightDamageWithoutShield();
            }
        }
    }
}

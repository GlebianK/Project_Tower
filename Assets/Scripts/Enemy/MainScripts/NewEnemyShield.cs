using UnityEngine;

public class NewEnemyShield : NewEnemyBase
{
    [SerializeField] private CombatAnimations combatAnimationsComponent;
    [SerializeField] private GameObject shieldObject;
    
    [Tooltip("Остаток здоровья для исчезновения щита (в процентах от максимального здоровья)")]
    [SerializeField, Range(0, 100)] private int dropShieldHPThreshold;

    private bool hasShield;

    public bool HasShield() => hasShield;

    private void Awake()
    {
        hasShield = true;
    }

    private void Update()
    {
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

    private void SubscribeEventsToLightDamageWithoutShield()
    {
        health.TookDamage.AddListener(combatController.OnAttackAborted);
        health.TookDamage.AddListener(combatAnimationsComponent.OnGetHit);
    }

    private void OnDisable()
    {
        health.TookDamage.RemoveListener(combatController.OnAttackAborted);
        health.TookDamage.RemoveListener(combatAnimationsComponent.OnGetHit);
    }

    public void DropShield()
    {
        if(TryGetComponent<Health>(out Health health))
        {
            Debug.Log($"Current Shield Enemy health: {health.HP}. Threshold: {health.MaxHP * (dropShieldHPThreshold / 100f)}");
            if(health.HP > (health.MaxHP * (dropShieldHPThreshold/100f)))
                return;

            if(shieldObject.activeSelf)
                shieldObject.SetActive(false);

            hasShield = false;
            SubscribeEventsToLightDamageWithoutShield();
        }        
    }
}

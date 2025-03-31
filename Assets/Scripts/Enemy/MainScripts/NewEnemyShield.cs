using UnityEngine;

public class NewEnemyShield : NewEnemyBase
{
    [SerializeField] private CombatAnimations combatAnimationsComponent;
    [SerializeField] private GameObject shieldObject;

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
            Debug.Log($"Current Shield Enemy health: {health.HP}. Half max: {health.MaxHP/2f}");
            if(health.HP > (health.MaxHP/2f))
                return;

            if(shieldObject.activeSelf)
                shieldObject.SetActive(false);

            hasShield = false;
            SubscribeEventsToLightDamageWithoutShield();
        }        
    }
}

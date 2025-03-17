using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackBase : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float damageDealingDelay;
    [SerializeField] protected float durationOfAttack;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected Transform AttackRaycastPointPosition;

    protected RaycastHit hit; 
    protected bool canAttack = true;

    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;

    public bool CanAttack() => canAttack;

    public void TryAttack()
    {
        if (CanAttack())
            StartCoroutine(PerformAttackCoroutine());
    }

    protected virtual IEnumerable<Health> CastAttackZone()
    {
        Debug.LogWarning("You're using base CastAttackZone method! Ensure using the overriden one.");
        yield return null;
    }

    protected virtual void ApplyDamageToHealthComponents(IEnumerable<Health> healthComponentsCollection)
    {
        if (healthComponentsCollection != null)
        {
            Debug.LogError("AttackBase.cs: YOU'RE CALLING THE OLD ATTACK METHOD WHICH IS NOT WORKING ANYMORE!");
            foreach (Health health in healthComponentsCollection)
            {
                /*
                if (health != null)
                    health.TakeDamage(damage);
                */
            }
        }
        else
        {
            Debug.LogError("AttackBase: No health collection to work with!");
        }
    }

    private IEnumerator PerformAttackCoroutine()
    {
        canAttack = false;
        AttackStarted.Invoke();

        yield return new WaitForSeconds(damageDealingDelay);

        IEnumerable<Health> healthComponentsCollection =  CastAttackZone();
        ApplyDamageToHealthComponents(healthComponentsCollection);

        yield return new WaitForSeconds(durationOfAttack - damageDealingDelay);

        AttackEnded.Invoke();
        canAttack = true;
    }
}

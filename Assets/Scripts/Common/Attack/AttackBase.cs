using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackBase : MonoBehaviour
{
    [SerializeField] private protected float damage;
    [SerializeField] private protected float attackRange;
    [SerializeField] private protected float damageDealingDelay;
    [SerializeField] private protected float durationOfAttack;
    [SerializeField] private protected LayerMask layerMask;
    [SerializeField] private protected Transform AttackRaycastPointPosition;

    private protected bool canAttack = true;

    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;

    public bool CanAttack() => canAttack;

    public void TryAttack()
    {
        if (CanAttack())
            StartCoroutine(PerformAttackCoroutine());
    }

    private protected virtual IEnumerable<Health> CastAttackZone()
    {
        Debug.LogWarning("You're using base CastAttackZone method! Ensure using the overriden one.");
        yield return null;
    }

    private void ApplyDamageToHealthComponents(IEnumerable<Health> healthComponentsCollection)
    {
        if (healthComponentsCollection != null)
        {
            foreach (Health health in healthComponentsCollection)
            {
                health.TakeDamage(damage);
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

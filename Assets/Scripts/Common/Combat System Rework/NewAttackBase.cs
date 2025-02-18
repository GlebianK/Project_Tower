using System.Collections.Generic;
using System.Threading.Tasks; // нужно для возможности задать поведение как у корутин, только без монобехов (т.е. без, собственно, корутин)
using UnityEngine;
using UnityEngine.Events;

public class NewAttackBase : ICombatAction
{
    [SerializeField] protected float damage;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float damageDealingDelay;
    [SerializeField] protected float durationOfAttack;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected Transform AttackRaycastPointPosition;
    [SerializeField] protected AttackType attackType;

    public UnityEvent AttackStarted;
    public UnityEvent AttackEnded;

    public enum AttackType
    {
        light,
        heavy
    }

    protected RaycastHit hit;
    protected bool canAttack = true;

    private async void PerformAttack()
    {
        canAttack = false;
        AttackStarted.Invoke();

        await Task.Delay((int)(damageDealingDelay * 1000)); // Delay считает в миллисекундах

        IEnumerable<Health> healthComponentsCollection = CastAttackZone();
        ApplyDamageToHealthComponents(healthComponentsCollection);

        await Task.Delay((int)((durationOfAttack - damageDealingDelay) * 1000));

        AttackEnded.Invoke();
        canAttack = true;
    }

    #region PROTECTED METHODS
    protected virtual IEnumerable<Health> CastAttackZone()
    {
        Debug.LogWarning("You're using base CastAttackZone method! Ensure using the overriden one.");
        yield return null;
    }

    protected virtual void ApplyDamageToHealthComponents(IEnumerable<Health> healthComponentsCollection)
    {
        if (healthComponentsCollection != null)
        {
            foreach (Health health in healthComponentsCollection)
            {
                if (health != null)
                    health.TakeDamage(damage * (1 - health.DamageReductionCoef));
            }
        }
        else
        {
            Debug.LogError("AttackBase: No health collection to work with!");
        }
    }
    #endregion

    #region PUBLIC METHODS FROM ICombatAction INTERFACE
    public bool CanPerform() => canAttack;

    public void Perform()
    {
        if (CanPerform())
        {
            PerformAttack();
        }
        
    }
    #endregion

}

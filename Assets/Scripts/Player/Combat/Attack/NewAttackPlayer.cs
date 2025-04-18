using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewAttackPlayer : NewAttackBase
{
    [Tooltip("Half the size of the casted hit box in each dimension")]
    [SerializeField] protected Vector3 boxCastSizes;

    protected bool collidersWereHit;

    protected override void ApplyDamageToHealthComponents(IEnumerable<Health> healthComponentsCollection)
    {
        if (healthComponentsCollection != null)
        {
            foreach (Health health in healthComponentsCollection)
            {
                if (health != null)
                {
                    if (health.gameObject.TryGetComponent<NewEnemyShield>(out NewEnemyShield nes)) // ���������� �� ����??
                    {
                        if(nes.HasShield() && attackType == AttackType.light)
                            continue;
                    }

                    health.TakeDamage(damage * (1 - health.DamageReductionCoef), attackType);
                }
            }
        }
        else
        {
            Debug.LogError("AttackBase: No health collection to work with!");
        }
    }

    protected override IEnumerable<Health> CastAttackZone()
    {
        Collider[] colliders = Physics.OverlapBox(
            AttackRaycastPointPosition.position + AttackRaycastPointPosition.forward * attackRange,
            boxCastSizes,
            AttackRaycastPointPosition.rotation,
            layerMask);

        foreach (Collider collider in colliders)
        {
            Debug.LogWarning("HIT !");
            if (collider.transform.TryGetComponent(out Health health))
            {
                Debug.Log("Player: object with Health component was hit!");
                Debug.Log($"if-state: collider name:{collider.name}");
                yield return health;
            }
        }
    }

    public virtual void OnAttackStarted()
    {
        Debug.Log("Player OnAttackStarted callback (base)");
    }

    public virtual void OnAttackEnded()
    {
        Debug.Log("Player OnAttackEnded callback (base)");
    }


    // ============================================================= GIZMOS ========================================= //

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = AttackRaycastPointPosition.localToWorldMatrix;
        //Check if there has been a hit yet
        if (collidersWereHit)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(Vector3.zero, Vector3.forward * hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(Vector3.forward * hit.distance, boxCastSizes * 2);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(Vector3.zero, Vector3.forward * attackRange);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(Vector3.forward * attackRange, boxCastSizes * 2);
        }
    }
}

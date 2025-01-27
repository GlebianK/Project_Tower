using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : AttackBase
{
    private protected override IEnumerable<Health> CastAttackZone()
    {
        Ray ray = new(AttackRaycastPointPosition.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * attackRange, Color.cyan);

        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, layerMask))
        {
            if (hit.transform.TryGetComponent<Health>(out Health health))
            {
                yield return health;
            }
        }
        else
        {
            Debug.LogError("AttackEnemy: No health components found!");
            yield return null;
        }
    }
}
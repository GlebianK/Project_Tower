using System.Collections.Generic;
using UnityEngine;

public class AttackEnemy : AttackBase
{
    protected override IEnumerable<Health> CastAttackZone()
    {
        Ray ray = new(AttackRaycastPointPosition.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * attackRange, Color.cyan);

        if (Physics.Raycast(ray, out hit, attackRange, layerMask))
        {
            if (hit.transform.TryGetComponent<Health>(out Health health))
            {
                yield return health;
            }
        }
        else
        {
            Debug.Log("AttackEnemy: No health components found!");
            yield return null;
        }
    }
}
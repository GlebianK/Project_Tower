using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewAttackEnemy : NewAttackBase
{
    protected override IEnumerable<Health> CastAttackZone()
    {
        if(AttackRaycastPointPosition != null)
        {
            Ray ray = new(AttackRaycastPointPosition.position, AttackRaycastPointPosition.transform.forward);
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
}

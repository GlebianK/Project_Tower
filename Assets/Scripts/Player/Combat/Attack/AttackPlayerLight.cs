using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerLight : AttackPlayerBase
{
    /*
    protected override IEnumerable<Health> CastAttackZone()
    {
        collidersWereHit = Physics.BoxCast(AttackRaycastPointPosition.position, boxCastSizes, transform.forward,
            out RaycastHit hit, Quaternion.identity, attackRange, layerMask);
        if (collidersWereHit)
        {
            Debug.LogWarning("HIT !");
            if (hit.transform.TryGetComponent<Health>(out Health health))
            {
                Debug.LogWarning("Object with Health component was hit!");
                yield return health;
            }
        }
        else
        {
            Debug.LogError("AttackEnemy: No health components found!");
            yield return null;
        }
    }
    */

    public override void DeactivateParentGameObject()
    {
        gameObject.SetActive(false);
    }
}

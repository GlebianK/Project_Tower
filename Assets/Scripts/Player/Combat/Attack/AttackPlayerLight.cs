using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerLight : AttackBase
{
    [Tooltip("Half the size of the casted hit box in each dimension")]
    [SerializeField] private Vector3 boxCastSizes;

    protected bool collidersWereHit;

    //TODO: добавить создание коллекции задетых объектов (каждый RaycastHit хранит инфу лишь об одном объекте)
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


    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (collidersWereHit)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * attackRange);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * attackRange, boxCastSizes);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * attackRange);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * attackRange, boxCastSizes);
        }
    }
    
}

using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerBase : AttackBase
{
    [SerializeField] private bool canBreakArmor;

    [Tooltip("Half the size of the casted hit box in each dimension")]
    [SerializeField] protected Vector3 boxCastSizes;

    protected bool collidersWereHit;

    //TODO: добавить создание коллекции задетых объектов (каждый RaycastHit хранит инфу лишь об одном объекте)
    protected override IEnumerable<Health> CastAttackZone()
    {

        //collidersWereHit = Physics.BoxCast(AttackRaycastPointPosition.position, boxCastSizes, transform.forward,
        //out hit, Quaternion.identity, maxDistance: attackRange, layerMask: layerMask);

        collidersWereHit = Physics.BoxCast(AttackRaycastPointPosition.position, boxCastSizes, AttackRaycastPointPosition.forward,
            out hit, AttackRaycastPointPosition.rotation, maxDistance: attackRange, layerMask: layerMask);
        Debug.Log($"hit:{(hit.distance).ToString()}, GENERAL TEXT");
        if (collidersWereHit)
        {
            Debug.LogWarning("HIT !");
            if (hit.transform.TryGetComponent<Health>(out Health health))
            {
                Debug.LogWarning("Object with Health component was hit!");
                Debug.Log($"if-stats1: range:{attackRange}, box:{boxCastSizes.x},{boxCastSizes.y},{boxCastSizes.z}, posZ:{AttackRaycastPointPosition.position.z}");
                Debug.Log($"if-stats2: hit:{hit.distance}, col:{hit.collider.name}, hitZ:{hit.point.z}");
                yield return health;
            }
        }
        else
        {
            Debug.LogError("AttackEnemy: No health components found!");
            Debug.Log($"else-stats: range:{attackRange}, box:{boxCastSizes.x},{boxCastSizes.x},{boxCastSizes.z}, posZ:{AttackRaycastPointPosition.position.z}");
            yield return null;
        }
        

        /*
        Debug.LogWarning("This is a base player CastAttackZone class, be sure to use the overriden one");
        return null;
        */
    }

    public virtual void DeactivateParentGameObject()
    {
        Debug.LogWarning("You're using base DeactivateParentGameObject method! Be sure to use the overriden one.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (collidersWereHit)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(AttackRaycastPointPosition.position, AttackRaycastPointPosition.forward * hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(AttackRaycastPointPosition.position + AttackRaycastPointPosition.forward * hit.distance, boxCastSizes * 2);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(AttackRaycastPointPosition.position, AttackRaycastPointPosition.forward * attackRange);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(AttackRaycastPointPosition.position + AttackRaycastPointPosition.forward * attackRange, boxCastSizes * 2);
        }
    }

}

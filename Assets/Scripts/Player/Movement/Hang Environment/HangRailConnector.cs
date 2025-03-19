using UnityEditor;
using UnityEngine;

public class HangRailConnector : MonoBehaviour
{
    [SerializeField] private LayerMask neighbourDetectionLayer;
    [SerializeField] private float searchRadius;
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
        
    }
    public HangRail GetNeighbour()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            searchRadius,
            neighbourDetectionLayer,
            QueryTriggerInteraction.Collide
            );

        HangRail parentRail = GetComponentInParent<HangRail>();
        if (parentRail == null )
        {
            Debug.LogError($"Hang rail connector isn't a child of Hang Rail\nError Object: {this}");
            return null;
        }


        foreach (Collider hit in colliders)
        {
            HangRail hangRail = hit.GetComponent<HangRail>();
            if (hangRail != null && hangRail != parentRail) 
            {
                return hangRail;
            }
        }

        return null;
    }
}

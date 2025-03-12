using UnityEngine;

public class IKClimbPointsPlacer : MonoBehaviour
{
    [SerializeField] private Transform climbPointsObject;

    public void OnStateChanged(PlayerMovementStateMachine machine, PlayerMovementStateType type)
    {
        if (!enabled) return;
        if (type == PlayerMovementStateType.Climb)
        {
            RaycastHit climbPointHit;
            machine.TryGetClimbPoint(out climbPointHit);
            Vector3 climbVector = climbPointHit.point - machine.transform.position;
            Debug.Log($"Climb Direction: {climbVector}");
            Vector3 raycastStart = machine.transform.position;
            raycastStart.y = climbPointHit.point.y - 0.05f;

            Vector3 raycastEnd = climbPointHit.point;
            raycastEnd.y -= 0.05f;

            Vector3 raycastDir = raycastEnd - raycastStart;
            float raycastRange = raycastDir.magnitude ;
            raycastDir = raycastDir.normalized;

            RaycastHit wallHit;

            Physics.Raycast(raycastStart, raycastDir, out wallHit, raycastRange + machine.cc.radius,
                machine.Properties.ClimbObstacleLayer,
                QueryTriggerInteraction.Ignore);

            //Vector3 forwardCheckVector = machine.transform.forward * machine.Properties.ClimbCheckForwardRange;

            //Vector3 cp1 = machine.transform.position + Vector3.up * (machine.cc.radius + machine.cc.skinWidth);
            //Vector3 cp2 = machine.transform.position + Vector3.up * (climbPointHit.point.y - machine.transform.position.y - machine.cc.radius);

            //Physics.CapsuleCast(
            //    cp1,
            //    cp2,
            //    0.1f,
            //    forwardCheckVector.normalized,
            //    out wallHit,
            //    forwardCheckVector.magnitude + 0.15f + machine.cc.radius,
            //    machine.Properties.ClimbObstacleLayer,
            //    QueryTriggerInteraction.Ignore);

            Vector3 wallNormal = wallHit.normal;

            climbPointsObject.position = climbPointHit.point;
            climbPointsObject.rotation = Quaternion.LookRotation(wallNormal, climbPointHit.normal);
        }
    }
}

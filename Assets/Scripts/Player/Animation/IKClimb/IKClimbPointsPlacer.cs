using UnityEngine;

public class IKClimbPointsPlacer : MonoBehaviour
{
    [SerializeField] private Transform climbPointsObject;

    public void OnStateChanged(PlayerMovementStateMachine machine, PlayerMovementStateType type)
    {
        if (!enabled) return;

        if (type == PlayerMovementStateType.Climb)
        {
            ClimbPlayerMovementState state = machine.GetCurrentState() as ClimbPlayerMovementState;
            RaycastHit climbPointHit = state.ClimbPointHit;

            Vector3 machinePosYAlligned = machine.transform.position;
            machinePosYAlligned.y = climbPointHit.point.y;
            Vector3 climbVector = climbPointHit.point - machine.transform.position;
            Debug.Log($"Climb Direction: {climbVector}");
            Vector3 raycastStart = machinePosYAlligned;
            raycastStart.y -= 0.05f;

            Vector3 raycastEnd = climbPointHit.point;
            raycastEnd.y -= 0.05f;

            Vector3 raycastDir = raycastEnd - raycastStart;
            float raycastRange = raycastDir.magnitude ;
            raycastDir = raycastDir.normalized;

            RaycastHit wallHit;

            bool successCast = Physics.Raycast(raycastStart, raycastDir, out wallHit, raycastRange + machine.cc.radius,
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
            climbPointsObject.rotation = Quaternion.LookRotation(-machine.transform.forward, climbPointHit.normal);

            if (successCast)
            {
                Debug.Log("Climb Rotated for wallHit");
                climbPointsObject.rotation = Quaternion.LookRotation(wallNormal, climbPointHit.normal);
            }

            //if (Vector3.Angle(climbPointsObject.forward, -machine.transform.forward) > 60)
            //{
            //    Debug.Log("Climb Rotation Corrected");
            //    climbPointsObject.rotation = Quaternion.LookRotation(-machine.transform.forward, climbPointHit.normal);

            //}
        }
    }
}

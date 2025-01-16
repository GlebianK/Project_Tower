using UnityEngine;

public class IKClimbPointsPlacer : MonoBehaviour
{
    [SerializeField] private Transform climbPointsObject;

    public void OnStateChanged(PlayerMovementStateMachine machine, PlayerMovementStateType type)
    {
        if (!enabled) return;
        if (type == PlayerMovementStateType.Climb)
        {
            RaycastHit climpPointHit;
            machine.TryGetClimbPoint(out climpPointHit);

            Vector3 forwardCheckVector = machine.transform.forward * machine.Properties.ClimbCheckForwardRange;

            RaycastHit wallHit;
            Physics.CapsuleCast(
                machine.transform.position + Vector3.up * (machine.cc.radius + machine.cc.skinWidth),
                machine.transform.position + Vector3.up * (machine.cc.height - machine.cc.radius),
                machine.cc.radius - 0.1f,
                forwardCheckVector.normalized,
                out wallHit,
                forwardCheckVector.magnitude,
                machine.Properties.ClimbObstacleLayer,
                QueryTriggerInteraction.Ignore);

            Vector3 wallNormal = wallHit.normal;

            climbPointsObject.position = climpPointHit.point + wallNormal * machine.cc.radius;
            climbPointsObject.rotation = Quaternion.LookRotation(wallNormal, climpPointHit.normal);
        }
    }
}

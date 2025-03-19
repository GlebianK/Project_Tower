using UnityEngine;
using UnityEngine.UIElements;

public class HangPlayerMovementState : PlayerMovementStateBase
{
    private HangRail rail;
    public HangPlayerMovementState(PlayerMovementStateConfig config) : 
        base(config)
    {
        rail = null;
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        Vector3 position = movementController.transform.position
            - (rail.forward * config.HangBodyOffset.z
                + rail.right * config.HangBodyOffset.x
                + Vector3.up * config.HangBodyOffset.y);

        position = rail.GetPositionOnRail(position);
        rail = rail.GetRailSegment(position);
        movementController.transform.position = position +
            (rail.forward * config.HangBodyOffset.z
            + rail.right * config.HangBodyOffset.x
            + Vector3.up * config.HangBodyOffset.y);
    }

    public override bool MakeTransitions(float deltaTime)
    {
        if (inputController.jumpPressed)
        {
            movementController.Jump(Vector3.up * config.JumpVelocity - rail.forward * config.HangHorizontalSpeed);
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }

        return false;
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        Vector3 position = movementController.transform.position;
        Vector3 direction = movementController.transform.forward * config.HangCheckOffset.z
            + movementController.transform.right * config.HangCheckOffset.x
            + movementController.transform.up * config.HangCheckOffset.y;
        if (Physics.Raycast(position,
            direction.normalized, 
            out RaycastHit hit,
            direction.magnitude,
            config.HangDetectionLayer, QueryTriggerInteraction.Collide))
        {
            rail = hit.transform.GetComponent<HangRail>();
            if (rail == null)
            {
                movementController.SetCurrentState(PlayerMovementStateType.Air);
                inputController.jumpPressed = false;
                return;
            }

            position -= movementController.transform.forward * config.HangBodyOffset.z
                + movementController.transform.right * config.HangBodyOffset.x
                + movementController.transform.up * config.HangBodyOffset.y;

            position = rail.GetPositionOnRail(position);
            movementController.transform.position = position;

            movementController.transform.rotation = Quaternion.LookRotation(rail.forward, Vector3.up);
            movementController.cc.enabled = false;

            inputController.jumpPressed = false;
        }
        else
        {
            movementController.SetCurrentState(PlayerMovementStateType.Air);
            inputController.jumpPressed = false;
        }
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        movementController.cc.enabled = true;
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        Vector3 input = inputController.GetMovementDirectionInTransformSpace(movementController.transform);

        if (input == Vector3.zero)
            return Vector3.zero;
        return Vector3.Project(input, rail.right).normalized * config.HangHorizontalSpeed;
    }
}

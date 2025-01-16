using UnityEngine;

public class ClimbPlayerMovementState : PlayerMovementStateBase
{
    private readonly float maxClimbRange;
    private readonly float climbSpeed;

    RaycastHit climbPointHit;

    private float climbTime;
    private Vector3 movementVector;
    public ClimbPlayerMovementState(PlayerMovementStateConfig config) : base(config)
    {
        maxClimbRange = (Vector3.forward * config.ClimbCheckForwardRange + Vector3.up * config.ClimbCheckpointHeight).magnitude;
        climbSpeed = maxClimbRange / config.ClimbMaxDuration;
    }

    public override void HandleObstacleAfterMovement(float deltaTime, in RaycastHit hit)
    {
        
    }

    public override bool MakeTransitions(float deltaTime)
    {
        if (climbTime >= config.ClimbMaxDuration)
        {

            movementController.transform.position = climbPointHit.point;

            movementController.SetCurrentState(PlayerMovementStateType.Crouch);
            movementController.GetCurrentState().UpdateMovementVelocity(deltaTime);
            return true;
        }
        return false;
    }

    public override void OnStateActivated(IPlayerMovementState prevState)
    {
        CharacterController cc = movementController.cc;

        movementController.cc.enabled = false;
        movementController.TryGetClimbPoint(out climbPointHit);

        movementVector = climbPointHit.point - cc.transform.position;

        float startTimeMinMaxed = 1 - movementVector.magnitude / maxClimbRange;

        climbTime = startTimeMinMaxed * config.ClimbMaxDuration;
    }

    public override void OnStateDeactivated(IPlayerMovementState nextState)
    {
        movementController.cc.enabled = true;
        movementController.CharacterVelocity = Vector3.zero;
    }

    protected override Vector3 ComputeVelocity(float deltaTime)
    {
        float currentSpeed = climbSpeed;
        climbTime += deltaTime;
        return movementVector.normalized * currentSpeed;
    }
}
